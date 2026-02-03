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
- **DataTemplates** (`DataTemplate.Triggers`) - with limitations, see below

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
| `Setter.TargetName` | ✅ Supported | Property must exist on `TargetType` |
| `Trigger.SourceName` | ✅ Supported | Property must exist on `TargetType` |
| `Condition.SourceName` in `MultiTrigger` | ✅ Supported | Property must exist on `TargetType` |
| `EventTrigger.SourceName` | ✅ Supported | |
| `DataTrigger` in templates | ✅ Supported | |
| `MultiTrigger` / `MultiDataTrigger` in templates | ✅ Supported | |

### DataTemplate Triggers

| Feature | Status | Notes |
|---------|--------|-------|
| Basic triggers without `TargetName` | ✅ Supported | |
| Triggers with `TargetName` | ⚠️ Limited | See compiler limitations below |

## Known Limitations

### Compiler Limitations

#### 1. Setter.Property Resolution with TargetName

**Issue**: The compiler resolves `Setter.Property` against the template's `TargetType`, not the element type specified by `TargetName`.

**Example that won't compile**:
```xml
<ControlTemplate TargetType="ContentControl">
    <TextBlock x:Name="statusText" Text="Hello"/>
    <ControlTemplate.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <!-- ERROR: 'Text' doesn't exist on ContentControl -->
            <Setter TargetName="statusText" Property="Text" Value="World"/>
        </Trigger>
    </ControlTemplate.Triggers>
</ControlTemplate>
```

**Workaround**: Only use properties in `Setter.Property` that exist on the template's `TargetType`. Inherited properties work (e.g., `Background` exists on `Control`, so it works for any control-derived template).

**Example that works**:
```xml
<ControlTemplate TargetType="Control">
    <Border x:Name="border" Background="White"/>
    <ControlTemplate.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <!-- OK: 'Background' exists on Control (inherited from Control) -->
            <Setter TargetName="border" Property="Background" Value="Blue"/>
        </Trigger>
    </ControlTemplate.Triggers>
</ControlTemplate>
```

#### 2. DataTemplate Triggers with TargetName

**Issue**: DataTemplates don't have a `TargetType`, so the compiler can't resolve properties like `Background` on named elements.

**Status**: DataTemplate triggers with `TargetName` are currently not usable until the compiler is enhanced.

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

Trigger values fit into the existing property value precedence system:

1. **Local Value** (highest)
2. **Trigger Value** ← Trigger setters apply here
3. **Style Value**
4. **Inherited Value**
5. **Default Value** (lowest)

Trigger values override style values but are overridden by local values.

## Testing

Test cases are located in:
- `src/Tests/TestApplication/TestApplication/Tests/TriggerTest.xaml`
- `src/Tests/TestApplication/TestApplication/Tests/TriggerTest.xaml.cs`

### Test Sections

| Section | Tests |
|---------|-------|
| 1. Property Triggers | `IsMouseOver`, `IsChecked`, `{x:Null}` |
| 2. DataTrigger | Boolean binding triggers |
| 3. MultiTrigger | AND logic with multiple conditions |
| 4. MultiDataTrigger | AND logic with multiple bindings |
| 5. EventTrigger | `MouseEnter`/`MouseLeave` with animations |
| 6.1 ControlTemplate with TargetName | Button template with hover/pressed states |
| 6.2 Trigger.SourceName | Monitors inner element, changes other elements |
| 6.3 Condition.SourceName | MultiTrigger with SourceName conditions |
| 6.4 EventTrigger.SourceName | Event on inner element triggers animation |
| 8. EnterActions/ExitActions | Animated transitions on trigger activation |
| 9. Style Inheritance | `BasedOn` styles with triggers |
| 10. Attached Property Triggers | `Grid.Row` trigger |
| 11. Programmatic Triggers | Code-behind style/trigger creation |

## Known Issues Under Investigation

### Template Triggers Not Working (Sections 6.1-6.4)

**Status**: Unresolved - requires further investigation

**Symptoms**:
- Hover triggers in ControlTemplates do NOT work (sections 6.1, 6.2, 6.3, 6.4)
- Click EventTrigger fires but Storyboard fails with name resolution error
- Style triggers (sections 1-5, 8, 9, 10) work correctly

**Error when clicking in section 6.4**:
```
System.InvalidOperationException: 'animatedArea' name cannot be found in the name scope of 'System.Windows.Controls.Border'.
   at System.Windows.Media.Animation.Storyboard.ResolveTargetName(...)
```

**What's working**:
- Style triggers are fully functional
- EventTrigger in templates DOES fire (the click handler executes)
- `TemplateTriggerStorage.ResolveNamedElement()` finds elements (e.g., "clickZone" for EventTrigger.SourceName)

**What's NOT working**:
- Property triggers (`IsMouseOver`, `IsPressed`) in templates don't fire
- `Storyboard.TargetName` resolution fails even though the name IS registered

**Investigation done**:
1. Generated code analysis shows triggers ARE being added to ControlTemplate
2. `XamlContext_SetTemplatedParent` and `XamlContext_RegisterName` are called correctly
3. Name scope is set via `FrameworkTemplate.SetTemplateNameScope()` before `TemplateChild` is set
4. `InitializeTemplateTriggerStorage()` should be called when `TemplateChild` is set
5. `_isProcessingTriggers` flag added to prevent re-entrancy
6. Fallback name scope lookup added in `ResolveNamedElement()`

**Likely root causes to investigate**:
1. **`InitializeTemplateTriggerStorage()` may not be called** - Verify with breakpoint/logging
2. **`OnPropertyChanged` may not reach `TemplateTriggerStorage`** - Check if `_templateTriggerStorage` is null when property changes
3. **Name scope timing** - The cached `_nameScope` may be null at construction
4. **`TemplatedParent` not set on template elements** - Would cause `FindName()` to use wrong scope
5. **Button's default template interference** - May need to check if custom template properly overrides default

**Key code paths to trace**:
1. `FrameworkElement.TemplateChild` setter → `InitializeTemplateTriggerStorage()`
2. `FrameworkElement.OnPropertyChanged()` → `_templateTriggerStorage?.OnPropertyChanged()`
3. `TemplateTriggerStorage.EvaluateTrigger()` → `ApplyTriggerSetters()`
4. `ResolveNamedElement()` → name scope lookup

**Test templates to use**:
- `CustomButtonTemplate` (section 6.1) - Button with IsMouseOver/IsPressed triggers
- `SourceNameTriggerTemplate` (section 6.2) - Control with Trigger.SourceName
- `EventTriggerSourceNameTemplate` (section 6.4) - Control with EventTrigger.SourceName and Storyboard

**Recommended next steps**:
1. Add `Console.WriteLine` or `Debug.WriteLine` in `InitializeTemplateTriggerStorage()` to verify it's called
2. Check if `TemplateInternal` returns non-null for the test controls
3. Verify `ct.HasTriggers` returns true for test templates
4. Add logging in `OnPropertyChanged()` to see if it's called when hovering
5. Check if `_propertyTriggerMap` contains entries for `IsMouseOver`/`IsPressed`

## Future Enhancements

### Compiler Enhancement Needed

To fully support `TargetName` with any property:

1. Parse the template to build a name-to-element-type mapping
2. When encountering `TargetName="xyz"`, look up the actual element type
3. Resolve `Setter.Property` against that element's type, not the template's `TargetType`

This would enable:
- `TextBlock.Text` setters in any template
- DataTemplate triggers with `TargetName`
- Full WPF compatibility for template triggers

### Other Potential Improvements

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
- For Storyboards in template triggers, ensure `TemplateChild` is used (not `_templatedParent`) so that `FindName()` resolves in the template's name scope

### Code Style

The implementation follows OpenSilver's existing patterns:
- Simple, readable code over premature optimization
- Consistent naming with WPF APIs
- Internal classes for implementation details
- XML documentation on public APIs
