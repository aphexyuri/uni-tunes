public class SCUIAction
{
	public enum ControlAction
	{
		None,
		Add,
		Remove,
		Play,
		Stop,
		MoveUp,
		MoveDown
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

	public void SetProps(ControlAction action, object data)
	{
		_action = action;
		_data = data;
	}
}
