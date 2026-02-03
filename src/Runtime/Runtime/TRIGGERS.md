# WPF Triggers Implementation in OpenSilver

This document describes the WPF Triggers implementation in OpenSilver, including supported features, known limitations, and guidance for maintainers.

## Overview

OpenSilver now supports WPF-style triggers including:
- **Property Triggers** (`Trigger`) - React to dependency property changes
- **Data Triggers** (`DataTrigger`) - React to data binding value changes
- **Multi-Triggers** (`MultiTrigger`) - React when multiple property conditions are met (AND logic)
- **Multi-Data Triggers** (`MultiDataTrigger`) - React when multiple binding conditions are met (AND logic)
- **Event Triggers** (`EventTrigger`) - React to routed events

Triggers can be defined in:
- **Styles** (`Style.Triggers`)
- **ControlTemplates** (`ControlTemplate.Triggers`)
- **DataTemplates** (`DataTemplate.Triggers`)

## Supported Features

### Style Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| `Trigger` with `Property` and `Value` | ✅ Supported | |
| `Trigger` with `{x:Null}` value | ✅ Supported | For nullable properties like `IsChecked` |
| `DataTrigger` with `Binding` and `Value` | ✅ Supported | |
| `MultiTrigger` with multiple `Condition`s | ✅ Supported | AND logic |
| `MultiDataTrigger` with multiple `Condition`s | ✅ Supported | AND logic |
| `EventTrigger` with `RoutedEvent` | ✅ Supported | |
| `Trigger.EnterActions` / `ExitActions` | ✅ Supported | With `BeginStoryboard` |
| Style inheritance (`BasedOn`) with triggers | ✅ Supported | Triggers from base styles are inherited |
| Triggers on attached properties (e.g., `Grid.Row`) | ✅ Supported | |

### ControlTemplate Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| `Trigger` in `ControlTemplate.Triggers` | ✅ Supported | |
| `Setter.TargetName` | ✅ Supported | Property resolved against named element's type |
| `Trigger.SourceName` | ✅ Supported | Monitors property on named element |
| `Condition.SourceName` in `MultiTrigger` | ✅ Supported | Monitors property on named element |
| `EventTrigger.SourceName` | ✅ Supported | Listens to event on named element |
| `DataTrigger` in templates | ✅ Supported | |
| `MultiTrigger` / `MultiDataTrigger` in templates | ✅ Supported | |

### DataTemplate Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| Basic triggers without `TargetName` | ✅ Supported | |
| Triggers with `TargetName` | ✅ Supported | Compiler resolves properties against named element's type |

## Compiler Enhancements

### TargetName Property Resolution (Implemented)

The compiler now supports resolving `Setter.Property` against the actual element type when `TargetName` is specified. This enables scenarios like:

```xml
<!-- This now compiles: Text is resolved against TextBlock -->
<ControlTemplate TargetType="ContentControl">
    <TextBlock x:Name="statusText" Text="Hello"/>
    <ControlTemplate.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter TargetName="statusText" Property="Text" Value="World"/>
        </Trigger>
    </ControlTemplate.Triggers>
</ControlTemplate>
```

**Implementation details**:
- The compiler searches template content for elements with matching `x:Name` or `Name`
- When a `Setter` has `TargetName`, the property is resolved against the named element's type
- This works for both `ControlTemplate.Triggers` and `DataTemplate.Triggers`

**Key code locations**:
- `src/Compiler/Compiler/2_ConvertingXamlToCSharp/GeneratingCSharpCode.Pass2.cs`: `TryGetNamedElementType()`, `FindNamedElement()`
- Same pattern in `GeneratingFSCode.Pass2.cs` and `GeneratingVBCode.Pass2.cs`

## Known Limitations

### Runtime Limitations

#### 1. Multiple Triggers Setting the Same Property

When multiple triggers are active and set the same property, the last-applied value wins. When one trigger becomes inactive, its value is cleared **without** checking if another active trigger should provide a value.

**Impact**: In rare edge cases where multiple triggers set the same property, deactivating one trigger may leave the property without a value even if another trigger is still active.

#### 2. Missing IsFocused Property

The `IsFocused` property is not available in OpenSilver on `FrameworkElement`. Use alternative properties like `IsEnabled` or handle focus through other means.

## Architecture

### Key Classes

| Class | Location | Purpose |
|-------|----------|---------|
| `Trigger` | `System.Windows/Trigger.cs` | Property-based trigger |
| `DataTrigger` | `System.Windows/DataTrigger.cs` | Data binding-based trigger |
| `MultiTrigger` | `System.Windows/MultiTrigger.cs` | Multiple property conditions |
| `MultiDataTrigger` | `System.Windows/MultiDataTrigger.cs` | Multiple binding conditions |
| `Condition` | `System.Windows/Condition.cs` | Condition for multi-triggers |
| `ConditionCollection` | `System.Windows/ConditionCollection.cs` | Collection of conditions |
| `StyleTriggerCollection` | `System.Windows/StyleTriggerCollection.cs` | Collection of triggers for styles/templates |
| `TriggerStorage` | `System.Windows/TriggerStorage.cs` | Per-instance state for style triggers |
| `TemplateTriggerStorage` | `System.Windows/TemplateTriggerStorage.cs` | Per-instance state for template triggers |

### How It Works

#### Style Triggers

1. When a `Style` is applied to a `FrameworkElement`, `StyleHelper.UpdateStyleCache()` is called
2. If the style has triggers, a `TriggerStorage` instance is created for the element
3. `TriggerStorage.Initialize()` sets up listeners for property changes and data bindings
4. When a monitored property/binding changes, `TriggerStorage.OnPropertyChanged()` or `OnDataTriggerValueChanged()` is called
5. The trigger is evaluated, and setters are applied/unapplied accordingly

#### Template Triggers

1. When a template is applied via `FrameworkElement.ApplyTemplate()`, the template content is created
2. If the template has triggers, a `TemplateTriggerStorage` instance is created
3. `TemplateTriggerStorage.Initialize()` sets up listeners, resolving `SourceName` elements via the template's name scope
4. When properties change, triggers are re-evaluated
5. Setters are applied to `TargetName` elements (resolved via the name scope)

### Property Value Precedence

OpenSilver uses a precedence system similar to WPF:

1. **ParentTemplateTrigger** (highest) - Template triggers with TargetName
2. **Local Value** - Values set directly via SetValue or XAML attributes
3. **Style Trigger** - Triggers in Style.Triggers
4. **Style Value** - Setters in Style.Setters
5. **Inherited Value** - Values inherited from parent elements
6. **Default Value** (lowest) - Default value from property metadata

**Key behavior**: Template triggers with `TargetName` (in ControlTemplate or DataTemplate) CAN override local values set on template elements. This matches WPF behavior.

## Testing

Test cases are located in:
- `src/Tests/TestApplication/TestApplication/Tests/TriggerTest.xaml`
- `src/Tests/TestApplication/TestApplication/Tests/TriggerTest.xaml.cs`

### Test Sections

| Section | Tests | Status |
|---------|-------|--------|
| 1. Property Triggers | `IsMouseOver`, `IsChecked`, `{x:Null}` | ✅ Working |
| 2. DataTrigger | Boolean binding triggers | ✅ Working |
| 3. MultiTrigger | AND logic with multiple conditions | ✅ Working |
| 4. MultiDataTrigger | AND logic with multiple bindings | ✅ Working |
| 5. EventTrigger | `MouseEnter`/`MouseLeave` with animations | ✅ Working |
| 6.1 ControlTemplate with TargetName | Button template with hover/pressed states | ✅ Working |
| 6.2 Trigger.SourceName | Monitors inner element, changes other elements | ✅ Working |
| 6.3 Condition.SourceName | MultiTrigger with SourceName conditions | ✅ Working |
| 6.4 EventTrigger.SourceName | Event on inner element triggers animation | ⚠️ Partial (click throws exception) |
| 7. DataTemplate Triggers | `DataTrigger` and `MultiDataTrigger` with `TargetName` in ItemsControl | ✅ Working |
| 8. EnterActions/ExitActions | Animated transitions on trigger activation | ✅ Working |
| 9. Style Inheritance | `BasedOn` styles with triggers | ✅ Working |
| 10. Attached Property Triggers | `Grid.Row` trigger | ✅ Working |
| 11. Programmatic Triggers | Code-behind style/trigger creation | ✅ Working |

## Known Issues

### Storyboard.TargetName in Template EventTriggers (Section 6.4)

**Status**: Unresolved - requires further investigation

**Symptom**: When clicking in section 6.4, the EventTrigger fires but the Storyboard fails with a name resolution error.

**Error**:
```
System.InvalidOperationException: 'animatedArea' name cannot be found in the name scope of 'System.Windows.Controls.Border'.
   at System.Windows.Media.Animation.Storyboard.ResolveTargetName(...)
```

**What's working**:
- Template property triggers (sections 6.1, 6.2, 6.3) now work correctly after the `ParentTemplateTrigger` precedence fix
- EventTrigger in templates DOES fire (the click handler executes)
- `TemplateTriggerStorage.ResolveNamedElement()` finds elements correctly

**What's NOT working**:
- `Storyboard.TargetName` resolution fails when the Storyboard is invoked via `BeginStoryboard` in a template trigger
- The Storyboard is passed `_templatedParent` but needs the template's name scope to resolve names

**Root cause**: 
The `InvokeEnterActions()` and `InvokeExitActions()` methods in `TemplateTriggerStorage` call `beginStoryboard.Storyboard?.Begin(_templatedParent)`. The `_templatedParent` is the control itself (e.g., a `Control`), not the template content. When the Storyboard tries to resolve `TargetName`, it uses the wrong name scope.

**Attempted fix**:
Changed to `beginStoryboard.Storyboard?.Begin(_templatedParent.TemplateChild ?? _templatedParent)` but this still fails because `TemplateChild` (a `Border` in this case) also doesn't have the correct name scope.

**Recommended next steps**:
1. Investigate how WPF passes the correct name scope to Storyboards in template triggers
2. Check if `FrameworkTemplate.GetTemplateNameScope()` can be used to get the correct scope
3. May need to modify `Storyboard.Begin()` to accept an `INameScope` parameter or find the scope differently

## Potential Future Enhancements

- Re-evaluate other active triggers when one deactivates (for overlapping setters)
- Lighter-weight `DataTriggerBindingHelper` implementation
- Support for `IsFocused` property

## Maintainer Notes

### Adding New Trigger Types

1. Create the trigger class inheriting from `TriggerBase`
2. Implement `Seal()` for validation and type conversion
3. Add handling in `TriggerStorage.SetupTrigger()` and `TemplateTriggerStorage.SetupTrigger()`
4. Add evaluation logic in the `EvaluateTriggerCondition()` methods

### Debugging Tips

- Check if triggers are being sealed (string-to-type conversion happens during sealing)
- Verify the name scope is populated when template triggers are initialized
- Use breakpoints in `OnPropertyChanged()` and `OnDataTriggerValueChanged()` to trace trigger evaluation
- Check `_triggerStates` dictionary to see which triggers are active
- For template triggers, values are applied with `ParentTemplateTrigger` precedence (highest), allowing them to override local values

### Code Style

The implementation follows OpenSilver's existing patterns:
- Simple, readable code over premature optimization
- Consistent naming with WPF APIs
- Internal classes for implementation details
- XML documentation on public APIs
