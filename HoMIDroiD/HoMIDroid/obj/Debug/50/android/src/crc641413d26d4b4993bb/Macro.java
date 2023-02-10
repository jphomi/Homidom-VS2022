package crc641413d26d4b4993bb;


public class Macro
	extends crc641413d26d4b4993bb.BaseObject
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HoMIDroid.BO.Macro, HoMIDroid", Macro.class, __md_methods);
	}


	public Macro ()
	{
		super ();
		if (getClass () == Macro.class)
			mono.android.TypeManager.Activate ("HoMIDroid.BO.Macro, HoMIDroid", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
