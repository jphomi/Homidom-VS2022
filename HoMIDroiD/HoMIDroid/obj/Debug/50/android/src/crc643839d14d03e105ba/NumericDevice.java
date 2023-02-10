package crc643839d14d03e105ba;


public class NumericDevice
	extends crc641413d26d4b4993bb.Device
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HoMIDroid.BO.Devices.NumericDevice, HoMIDroid", NumericDevice.class, __md_methods);
	}


	public NumericDevice ()
	{
		super ();
		if (getClass () == NumericDevice.class)
			mono.android.TypeManager.Activate ("HoMIDroid.BO.Devices.NumericDevice, HoMIDroid", "", this, new java.lang.Object[] {  });
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
