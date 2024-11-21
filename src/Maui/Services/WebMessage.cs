namespace OpenSilver.Maui;

public class WebMessage
{
    public string MethodName { get; set; }
    public int CallbackId { get; set; }
    public string IdWhereCallbackArgsAreStored { get; set; }

    public object[] CallbackArgsObject { get; set; }

    public bool ReturnValue { get; set; }
}