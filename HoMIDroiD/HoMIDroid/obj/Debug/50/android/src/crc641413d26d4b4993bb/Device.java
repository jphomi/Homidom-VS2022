package crc641413d26d4b4993bb;


public class Device
	extends crc641413d26d4b4993bb.BaseObject
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HoMIDroid.BO.Device, HoMIDroid", Device.class, __md_methods);
	}


	public Device ()
	{
		super ();
		if (getClass () == Device.class)
			mono.android.TypeManager.Activate ("HoMIDroid.BO.Device, HoMIDroid", "", this, new java.lang.Object[] {  });
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
