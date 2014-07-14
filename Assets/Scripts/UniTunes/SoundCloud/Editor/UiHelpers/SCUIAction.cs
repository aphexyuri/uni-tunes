public class SCUIAction
{
	public enum ControlAction
	{
		None,
		Add,
		Remove,
		Play
	}

	private ControlAction _action = ControlAction.None;

	private object _data;

	public SCUIAction(ControlAction controlAction, object additionalData)
	{
		_action = controlAction;
		_data = additionalData;
	}

	public ControlAction Action
	{
		get { return _action; }
		set { _action = value; }
	}

	public object Data
	{
		get { return _data; }
		set { _data = value; }
	}
}
