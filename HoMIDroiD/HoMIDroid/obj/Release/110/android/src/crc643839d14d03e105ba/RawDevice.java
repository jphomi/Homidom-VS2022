package crc643839d14d03e105ba;


public class RawDevice
	extends crc641413d26d4b4993bb.Device
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HoMIDroid.BO.Devices.RawDevice, HoMIDroid", RawDevice.class, __md_methods);
	}


	public RawDevice ()
	{
		super ();
		if (getClass () == RawDevice.class)
			mono.android.TypeManager.Activate ("HoMIDroid.BO.Devices.RawDevice, HoMIDroid", "", this, new java.lang.Object[] {  });
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
