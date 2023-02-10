package crc641413d26d4b4993bb;


public class Group_1
	extends crc641413d26d4b4993bb.BaseObject
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HoMIDroid.BO.Group`1, HoMIDroid", Group_1.class, __md_methods);
	}


	public Group_1 ()
	{
		super ();
		if (getClass () == Group_1.class)
			mono.android.TypeManager.Activate ("HoMIDroid.BO.Group`1, HoMIDroid", "", this, new java.lang.Object[] {  });
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
