package crc6413c9890f0694d7d9;


public class ObjectRef_1
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("HoMIDroid.ObjectRef`1, HoMIDroid", ObjectRef_1.class, __md_methods);
	}


	public ObjectRef_1 ()
	{
		super ();
		if (getClass () == ObjectRef_1.class)
			mono.android.TypeManager.Activate ("HoMIDroid.ObjectRef`1, HoMIDroid", "", this, new java.lang.Object[] {  });
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
