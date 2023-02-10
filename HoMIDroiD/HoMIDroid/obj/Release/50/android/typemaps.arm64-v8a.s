	.arch	armv8-a
	.file	"typemaps.arm64-v8a.s"

/* map_module_count: START */
	.section	.rodata.map_module_count,"a",@progbits
	.type	map_module_count, @object
	.p2align	2
	.global	map_module_count
map_module_count:
	.size	map_module_count, 4
	.word	2
/* map_module_count: END */

/* java_type_count: START */
	.section	.rodata.java_type_count,"a",@progbits
	.type	java_type_count, @object
	.p2align	2
	.global	java_type_count
java_type_count:
	.size	java_type_count, 4
	.word	239
/* java_type_count: END */

	.include	"typemaps.shared.inc"
	.include	"typemaps.arm64-v8a-managed.inc"

/* Managed to Java map: START */
	.section	.data.rel.map_modules,"aw",@progbits
	.type	map_modules, @object
	.p2align	3
	.global	map_modules
map_modules:
	/* module_uuid: 1b53a907-26b5-4dd8-83df-e82ec8c47fe3 */
	.byte	0x07, 0xa9, 0x53, 0x1b, 0xb5, 0x26, 0xd8, 0x4d, 0x83, 0xdf, 0xe8, 0x2e, 0xc8, 0xc4, 0x7f, 0xe3
	/* entry_count */
	.word	28
	/* duplicate_count */
	.word	0
	/* map */
	.xword	module0_managed_to_java
	/* duplicate_map */
	.xword	0
	/* assembly_name: HoMIDroid */
	.xword	.L.map_aname.0
	/* image */
	.xword	0
	/* java_name_width */
	.word	0
	/* java_map */
	.zero	4
	.xword	0

	/* module_uuid: b270e32f-567b-4308-ac9b-e9522a3d968e */
	.byte	0x2f, 0xe3, 0x70, 0xb2, 0x7b, 0x56, 0x08, 0x43, 0xac, 0x9b, 0xe9, 0x52, 0x2a, 0x3d, 0x96, 0x8e
	/* entry_count */
	.word	211
	/* duplicate_count */
	.word	113
	/* map */
	.xword	module1_managed_to_java
	/* duplicate_map */
	.xword	module1_managed_to_java_duplicates
	/* assembly_name: Mono.Android */
	.xword	.L.map_aname.1
	/* image */
	.xword	0
	/* java_name_width */
	.word	0
	/* java_map */
	.zero	4
	.xword	0

	.size	map_modules, 144
/* Managed to Java map: END */

/* Java to managed map: START */
	.section	.rodata.map_java,"a",@progbits
	.type	map_java, @object
	.p2align	2
	.global	map_java
map_java:
	/* #0 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554744
	/* java_name */
	.ascii	"android/app/Activity"
	.zero	55
	.zero	3

	/* #1 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554753
	/* java_name */
	.ascii	"android/app/ActivityGroup"
	.zero	50
	.zero	3

	/* #2 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554745
	/* java_name */
	.ascii	"android/app/AlertDialog"
	.zero	52
	.zero	3

	/* #3 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554746
	/* java_name */
	.ascii	"android/app/AlertDialog$Builder"
	.zero	44
	.zero	3

	/* #4 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554747
	/* java_name */
	.ascii	"android/app/Application"
	.zero	52
	.zero	3

	/* #5 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554748
	/* java_name */
	.ascii	"android/app/Dialog"
	.zero	57
	.zero	3

	/* #6 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554754
	/* java_name */
	.ascii	"android/app/ExpandableListActivity"
	.zero	41
	.zero	3

	/* #7 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554755
	/* java_name */
	.ascii	"android/app/ListActivity"
	.zero	51
	.zero	3

	/* #8 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554749
	/* java_name */
	.ascii	"android/app/ProgressDialog"
	.zero	49
	.zero	3

	/* #9 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554756
	/* java_name */
	.ascii	"android/app/TabActivity"
	.zero	52
	.zero	3

	/* #10 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/ComponentCallbacks"
	.zero	41
	.zero	3

	/* #11 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/ComponentCallbacks2"
	.zero	40
	.zero	3

	/* #12 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554759
	/* java_name */
	.ascii	"android/content/ComponentName"
	.zero	46
	.zero	3

	/* #13 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554757
	/* java_name */
	.ascii	"android/content/Context"
	.zero	52
	.zero	3

	/* #14 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554761
	/* java_name */
	.ascii	"android/content/ContextWrapper"
	.zero	45
	.zero	3

	/* #15 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/DialogInterface"
	.zero	44
	.zero	3

	/* #16 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/DialogInterface$OnClickListener"
	.zero	28
	.zero	3

	/* #17 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/DialogInterface$OnMultiChoiceClickListener"
	.zero	17
	.zero	3

	/* #18 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554758
	/* java_name */
	.ascii	"android/content/Intent"
	.zero	53
	.zero	3

	/* #19 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/SharedPreferences"
	.zero	42
	.zero	3

	/* #20 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/SharedPreferences$Editor"
	.zero	35
	.zero	3

	/* #21 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/content/SharedPreferences$OnSharedPreferenceChangeListener"
	.zero	9
	.zero	3

	/* #22 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554785
	/* java_name */
	.ascii	"android/content/res/Configuration"
	.zero	42
	.zero	3

	/* #23 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554786
	/* java_name */
	.ascii	"android/content/res/Resources"
	.zero	46
	.zero	3

	/* #24 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554543
	/* java_name */
	.ascii	"android/database/DataSetObserver"
	.zero	43
	.zero	3

	/* #25 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554736
	/* java_name */
	.ascii	"android/graphics/Point"
	.zero	53
	.zero	3

	/* #26 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554737
	/* java_name */
	.ascii	"android/graphics/Rect"
	.zero	54
	.zero	3

	/* #27 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554738
	/* java_name */
	.ascii	"android/graphics/drawable/Drawable"
	.zero	41
	.zero	3

	/* #28 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/graphics/drawable/Drawable$Callback"
	.zero	32
	.zero	3

	/* #29 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554728
	/* java_name */
	.ascii	"android/os/BaseBundle"
	.zero	54
	.zero	3

	/* #30 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554729
	/* java_name */
	.ascii	"android/os/Bundle"
	.zero	58
	.zero	3

	/* #31 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554727
	/* java_name */
	.ascii	"android/os/Handler"
	.zero	57
	.zero	3

	/* #32 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554732
	/* java_name */
	.ascii	"android/os/Looper"
	.zero	58
	.zero	3

	/* #33 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554733
	/* java_name */
	.ascii	"android/os/Parcel"
	.zero	58
	.zero	3

	/* #34 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/os/Parcelable"
	.zero	54
	.zero	3

	/* #35 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554832
	/* java_name */
	.ascii	"android/runtime/JavaProxyThrowable"
	.zero	41
	.zero	3

	/* #36 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/text/Editable"
	.zero	54
	.zero	3

	/* #37 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/text/GetChars"
	.zero	54
	.zero	3

	/* #38 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/text/InputFilter"
	.zero	51
	.zero	3

	/* #39 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/text/NoCopySpan"
	.zero	52
	.zero	3

	/* #40 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/text/Spannable"
	.zero	53
	.zero	3

	/* #41 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/text/Spanned"
	.zero	55
	.zero	3

	/* #42 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/text/TextWatcher"
	.zero	51
	.zero	3

	/* #43 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/util/AttributeSet"
	.zero	50
	.zero	3

	/* #44 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554705
	/* java_name */
	.ascii	"android/util/DisplayMetrics"
	.zero	48
	.zero	3

	/* #45 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554704
	/* java_name */
	.ascii	"android/util/Log"
	.zero	59
	.zero	3

	/* #46 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554651
	/* java_name */
	.ascii	"android/view/ActionMode"
	.zero	52
	.zero	3

	/* #47 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ActionMode$Callback"
	.zero	43
	.zero	3

	/* #48 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554655
	/* java_name */
	.ascii	"android/view/ActionProvider"
	.zero	48
	.zero	3

	/* #49 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ContextMenu"
	.zero	51
	.zero	3

	/* #50 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ContextMenu$ContextMenuInfo"
	.zero	35
	.zero	3

	/* #51 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554657
	/* java_name */
	.ascii	"android/view/ContextThemeWrapper"
	.zero	43
	.zero	3

	/* #52 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554658
	/* java_name */
	.ascii	"android/view/Display"
	.zero	55
	.zero	3

	/* #53 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554673
	/* java_name */
	.ascii	"android/view/InputEvent"
	.zero	52
	.zero	3

	/* #54 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554632
	/* java_name */
	.ascii	"android/view/KeyEvent"
	.zero	54
	.zero	3

	/* #55 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/KeyEvent$Callback"
	.zero	45
	.zero	3

	/* #56 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554635
	/* java_name */
	.ascii	"android/view/LayoutInflater"
	.zero	48
	.zero	3

	/* #57 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/LayoutInflater$Factory"
	.zero	40
	.zero	3

	/* #58 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/LayoutInflater$Factory2"
	.zero	39
	.zero	3

	/* #59 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/Menu"
	.zero	58
	.zero	3

	/* #60 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/MenuItem"
	.zero	54
	.zero	3

	/* #61 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/MenuItem$OnActionExpandListener"
	.zero	31
	.zero	3

	/* #62 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/MenuItem$OnMenuItemClickListener"
	.zero	30
	.zero	3

	/* #63 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554640
	/* java_name */
	.ascii	"android/view/MotionEvent"
	.zero	51
	.zero	3

	/* #64 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/SubMenu"
	.zero	55
	.zero	3

	/* #65 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554618
	/* java_name */
	.ascii	"android/view/View"
	.zero	58
	.zero	3

	/* #66 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/View$OnClickListener"
	.zero	42
	.zero	3

	/* #67 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/View$OnCreateContextMenuListener"
	.zero	30
	.zero	3

	/* #68 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/View$OnKeyListener"
	.zero	44
	.zero	3

	/* #69 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554694
	/* java_name */
	.ascii	"android/view/ViewGroup"
	.zero	53
	.zero	3

	/* #70 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554695
	/* java_name */
	.ascii	"android/view/ViewGroup$LayoutParams"
	.zero	40
	.zero	3

	/* #71 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ViewManager"
	.zero	51
	.zero	3

	/* #72 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ViewParent"
	.zero	52
	.zero	3

	/* #73 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554641
	/* java_name */
	.ascii	"android/view/ViewTreeObserver"
	.zero	46
	.zero	3

	/* #74 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ViewTreeObserver$OnGlobalLayoutListener"
	.zero	23
	.zero	3

	/* #75 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ViewTreeObserver$OnPreDrawListener"
	.zero	28
	.zero	3

	/* #76 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/ViewTreeObserver$OnTouchModeChangeListener"
	.zero	20
	.zero	3

	/* #77 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554648
	/* java_name */
	.ascii	"android/view/Window"
	.zero	56
	.zero	3

	/* #78 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/Window$Callback"
	.zero	47
	.zero	3

	/* #79 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/WindowManager"
	.zero	49
	.zero	3

	/* #80 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554681
	/* java_name */
	.ascii	"android/view/WindowManager$LayoutParams"
	.zero	36
	.zero	3

	/* #81 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554698
	/* java_name */
	.ascii	"android/view/accessibility/AccessibilityEvent"
	.zero	30
	.zero	3

	/* #82 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/view/accessibility/AccessibilityEventSource"
	.zero	24
	.zero	3

	/* #83 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554699
	/* java_name */
	.ascii	"android/view/accessibility/AccessibilityRecord"
	.zero	29
	.zero	3

	/* #84 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554545
	/* java_name */
	.ascii	"android/widget/AbsListView"
	.zero	49
	.zero	3

	/* #85 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554558
	/* java_name */
	.ascii	"android/widget/AbsSeekBar"
	.zero	50
	.zero	3

	/* #86 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/Adapter"
	.zero	53
	.zero	3

	/* #87 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554547
	/* java_name */
	.ascii	"android/widget/AdapterView"
	.zero	49
	.zero	3

	/* #88 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/AdapterView$OnItemClickListener"
	.zero	29
	.zero	3

	/* #89 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/ArrayAdapter"
	.zero	48
	.zero	3

	/* #90 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554562
	/* java_name */
	.ascii	"android/widget/BaseAdapter"
	.zero	49
	.zero	3

	/* #91 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554564
	/* java_name */
	.ascii	"android/widget/BaseExpandableListAdapter"
	.zero	35
	.zero	3

	/* #92 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554566
	/* java_name */
	.ascii	"android/widget/Button"
	.zero	54
	.zero	3

	/* #93 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/Checkable"
	.zero	51
	.zero	3

	/* #94 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554567
	/* java_name */
	.ascii	"android/widget/CompoundButton"
	.zero	46
	.zero	3

	/* #95 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/CompoundButton$OnCheckedChangeListener"
	.zero	22
	.zero	3

	/* #96 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554575
	/* java_name */
	.ascii	"android/widget/EditText"
	.zero	52
	.zero	3

	/* #97 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/ExpandableListAdapter"
	.zero	39
	.zero	3

	/* #98 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554576
	/* java_name */
	.ascii	"android/widget/ExpandableListView"
	.zero	42
	.zero	3

	/* #99 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/ExpandableListView$OnChildClickListener"
	.zero	21
	.zero	3

	/* #100 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/ExpandableListView$OnGroupCollapseListener"
	.zero	18
	.zero	3

	/* #101 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/ExpandableListView$OnGroupExpandListener"
	.zero	20
	.zero	3

	/* #102 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554587
	/* java_name */
	.ascii	"android/widget/Filter"
	.zero	54
	.zero	3

	/* #103 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/Filter$FilterListener"
	.zero	39
	.zero	3

	/* #104 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/Filterable"
	.zero	50
	.zero	3

	/* #105 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554591
	/* java_name */
	.ascii	"android/widget/FrameLayout"
	.zero	49
	.zero	3

	/* #106 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/HeterogeneousExpandableList"
	.zero	33
	.zero	3

	/* #107 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554604
	/* java_name */
	.ascii	"android/widget/ImageButton"
	.zero	49
	.zero	3

	/* #108 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554605
	/* java_name */
	.ascii	"android/widget/ImageView"
	.zero	51
	.zero	3

	/* #109 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/ListAdapter"
	.zero	49
	.zero	3

	/* #110 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554608
	/* java_name */
	.ascii	"android/widget/ListView"
	.zero	52
	.zero	3

	/* #111 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554609
	/* java_name */
	.ascii	"android/widget/ProgressBar"
	.zero	49
	.zero	3

	/* #112 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554610
	/* java_name */
	.ascii	"android/widget/SeekBar"
	.zero	53
	.zero	3

	/* #113 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/SeekBar$OnSeekBarChangeListener"
	.zero	29
	.zero	3

	/* #114 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"android/widget/SpinnerAdapter"
	.zero	46
	.zero	3

	/* #115 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554613
	/* java_name */
	.ascii	"android/widget/TabHost"
	.zero	53
	.zero	3

	/* #116 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554614
	/* java_name */
	.ascii	"android/widget/TabHost$TabSpec"
	.zero	45
	.zero	3

	/* #117 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554556
	/* java_name */
	.ascii	"android/widget/TextView"
	.zero	52
	.zero	3

	/* #118 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554557
	/* java_name */
	.ascii	"android/widget/TextView$BufferType"
	.zero	41
	.zero	3

	/* #119 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554615
	/* java_name */
	.ascii	"android/widget/Toast"
	.zero	55
	.zero	3

	/* #120 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554617
	/* java_name */
	.ascii	"android/widget/ToggleButton"
	.zero	48
	.zero	3

	/* #121 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554453
	/* java_name */
	.ascii	"crc6413c9890f0694d7d9/HmdApp"
	.zero	47
	.zero	3

	/* #122 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554450
	/* java_name */
	.ascii	"crc6413c9890f0694d7d9/ListDevice2"
	.zero	42
	.zero	3

	/* #123 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554452
	/* java_name */
	.ascii	"crc6413c9890f0694d7d9/ListDevices"
	.zero	42
	.zero	3

	/* #124 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554451
	/* java_name */
	.ascii	"crc6413c9890f0694d7d9/MainActivity"
	.zero	41
	.zero	3

	/* #125 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"crc6413c9890f0694d7d9/ObjectRef_1"
	.zero	42
	.zero	3

	/* #126 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554769
	/* java_name */
	.ascii	"crc641413d26d4b4993bb/BaseObject"
	.zero	43
	.zero	3

	/* #127 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554770
	/* java_name */
	.ascii	"crc641413d26d4b4993bb/Device"
	.zero	47
	.zero	3

	/* #128 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"crc641413d26d4b4993bb/Group_1"
	.zero	46
	.zero	3

	/* #129 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554785
	/* java_name */
	.ascii	"crc641413d26d4b4993bb/Macro"
	.zero	48
	.zero	3

	/* #130 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554786
	/* java_name */
	.ascii	"crc641413d26d4b4993bb/Zone"
	.zero	49
	.zero	3

	/* #131 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554788
	/* java_name */
	.ascii	"crc643839d14d03e105ba/NumericDevice"
	.zero	40
	.zero	3

	/* #132 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554789
	/* java_name */
	.ascii	"crc643839d14d03e105ba/OnOffDevice"
	.zero	42
	.zero	3

	/* #133 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554787
	/* java_name */
	.ascii	"crc643839d14d03e105ba/RawDevice"
	.zero	44
	.zero	3

	/* #134 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554796
	/* java_name */
	.ascii	"crc645c35363e850b5894/ConnectionManager"
	.zero	36
	.zero	3

	/* #135 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554797
	/* java_name */
	.ascii	"crc645c35363e850b5894/DeviceDim"
	.zero	44
	.zero	3

	/* #136 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554846
	/* java_name */
	.ascii	"crc645c35363e850b5894/DeviceDim_CustomSeekBarChangeListener"
	.zero	16
	.zero	3

	/* #137 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554798
	/* java_name */
	.ascii	"crc645c35363e850b5894/DeviceOnOff"
	.zero	42
	.zero	3

	/* #138 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554801
	/* java_name */
	.ascii	"crc645c35363e850b5894/ListGroupDevice"
	.zero	38
	.zero	3

	/* #139 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554799
	/* java_name */
	.ascii	"crc645c35363e850b5894/ListMacro"
	.zero	44
	.zero	3

	/* #140 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554803
	/* java_name */
	.ascii	"crc645c35363e850b5894/ListZone"
	.zero	45
	.zero	3

	/* #141 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554800
	/* java_name */
	.ascii	"crc645c35363e850b5894/Login"
	.zero	48
	.zero	3

	/* #142 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554802
	/* java_name */
	.ascii	"crc645c35363e850b5894/ZoneContent"
	.zero	42
	.zero	3

	/* #143 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"crc64da21ca3ef8c6d683/BaseExpandableGroupAdapter_1"
	.zero	25
	.zero	3

	/* #144 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554795
	/* java_name */
	.ascii	"crc64da21ca3ef8c6d683/DeviceAdapter"
	.zero	40
	.zero	3

	/* #145 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554794
	/* java_name */
	.ascii	"crc64da21ca3ef8c6d683/DeviceExpandableGroupAdapter"
	.zero	25
	.zero	3

	/* #146 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554791
	/* java_name */
	.ascii	"crc64da21ca3ef8c6d683/MacroAdapter"
	.zero	41
	.zero	3

	/* #147 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554792
	/* java_name */
	.ascii	"crc64da21ca3ef8c6d683/ZoneAdapter"
	.zero	42
	.zero	3

	/* #148 */
	/* module_index */
	.word	0
	/* type_token_id */
	.word	33554793
	/* java_name */
	.ascii	"crc64da21ca3ef8c6d683/ZoneContentExpandableGroupAdapter"
	.zero	20
	.zero	3

	/* #149 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/io/Closeable"
	.zero	58
	.zero	3

	/* #150 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554948
	/* java_name */
	.ascii	"java/io/FileInputStream"
	.zero	52
	.zero	3

	/* #151 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/io/Flushable"
	.zero	58
	.zero	3

	/* #152 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554955
	/* java_name */
	.ascii	"java/io/IOException"
	.zero	56
	.zero	3

	/* #153 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554953
	/* java_name */
	.ascii	"java/io/InputStream"
	.zero	56
	.zero	3

	/* #154 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554958
	/* java_name */
	.ascii	"java/io/OutputStream"
	.zero	55
	.zero	3

	/* #155 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554960
	/* java_name */
	.ascii	"java/io/PrintWriter"
	.zero	56
	.zero	3

	/* #156 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/io/Serializable"
	.zero	55
	.zero	3

	/* #157 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554961
	/* java_name */
	.ascii	"java/io/StringWriter"
	.zero	55
	.zero	3

	/* #158 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554962
	/* java_name */
	.ascii	"java/io/Writer"
	.zero	61
	.zero	3

	/* #159 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/Appendable"
	.zero	55
	.zero	3

	/* #160 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554899
	/* java_name */
	.ascii	"java/lang/Boolean"
	.zero	58
	.zero	3

	/* #161 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554900
	/* java_name */
	.ascii	"java/lang/Byte"
	.zero	61
	.zero	3

	/* #162 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/CharSequence"
	.zero	53
	.zero	3

	/* #163 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554901
	/* java_name */
	.ascii	"java/lang/Character"
	.zero	56
	.zero	3

	/* #164 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554902
	/* java_name */
	.ascii	"java/lang/Class"
	.zero	60
	.zero	3

	/* #165 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554918
	/* java_name */
	.ascii	"java/lang/ClassCastException"
	.zero	47
	.zero	3

	/* #166 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554903
	/* java_name */
	.ascii	"java/lang/ClassNotFoundException"
	.zero	43
	.zero	3

	/* #167 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/Cloneable"
	.zero	56
	.zero	3

	/* #168 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/Comparable"
	.zero	55
	.zero	3

	/* #169 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554904
	/* java_name */
	.ascii	"java/lang/Double"
	.zero	59
	.zero	3

	/* #170 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554919
	/* java_name */
	.ascii	"java/lang/Enum"
	.zero	61
	.zero	3

	/* #171 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554921
	/* java_name */
	.ascii	"java/lang/Error"
	.zero	60
	.zero	3

	/* #172 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554905
	/* java_name */
	.ascii	"java/lang/Exception"
	.zero	56
	.zero	3

	/* #173 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554906
	/* java_name */
	.ascii	"java/lang/Float"
	.zero	60
	.zero	3

	/* #174 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554930
	/* java_name */
	.ascii	"java/lang/IllegalArgumentException"
	.zero	41
	.zero	3

	/* #175 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554931
	/* java_name */
	.ascii	"java/lang/IllegalStateException"
	.zero	44
	.zero	3

	/* #176 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554932
	/* java_name */
	.ascii	"java/lang/IndexOutOfBoundsException"
	.zero	40
	.zero	3

	/* #177 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554908
	/* java_name */
	.ascii	"java/lang/Integer"
	.zero	58
	.zero	3

	/* #178 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554935
	/* java_name */
	.ascii	"java/lang/LinkageError"
	.zero	53
	.zero	3

	/* #179 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554909
	/* java_name */
	.ascii	"java/lang/Long"
	.zero	61
	.zero	3

	/* #180 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554936
	/* java_name */
	.ascii	"java/lang/NoClassDefFoundError"
	.zero	45
	.zero	3

	/* #181 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554937
	/* java_name */
	.ascii	"java/lang/NullPointerException"
	.zero	45
	.zero	3

	/* #182 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554938
	/* java_name */
	.ascii	"java/lang/Number"
	.zero	59
	.zero	3

	/* #183 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554910
	/* java_name */
	.ascii	"java/lang/Object"
	.zero	59
	.zero	3

	/* #184 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554940
	/* java_name */
	.ascii	"java/lang/ReflectiveOperationException"
	.zero	37
	.zero	3

	/* #185 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/Runnable"
	.zero	57
	.zero	3

	/* #186 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554911
	/* java_name */
	.ascii	"java/lang/RuntimeException"
	.zero	49
	.zero	3

	/* #187 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554912
	/* java_name */
	.ascii	"java/lang/Short"
	.zero	60
	.zero	3

	/* #188 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554913
	/* java_name */
	.ascii	"java/lang/String"
	.zero	59
	.zero	3

	/* #189 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554915
	/* java_name */
	.ascii	"java/lang/Thread"
	.zero	59
	.zero	3

	/* #190 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554917
	/* java_name */
	.ascii	"java/lang/Throwable"
	.zero	56
	.zero	3

	/* #191 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554941
	/* java_name */
	.ascii	"java/lang/UnsupportedOperationException"
	.zero	36
	.zero	3

	/* #192 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/reflect/GenericDeclaration"
	.zero	39
	.zero	3

	/* #193 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/reflect/Type"
	.zero	53
	.zero	3

	/* #194 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/lang/reflect/TypeVariable"
	.zero	45
	.zero	3

	/* #195 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554856
	/* java_name */
	.ascii	"java/net/InetSocketAddress"
	.zero	49
	.zero	3

	/* #196 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554857
	/* java_name */
	.ascii	"java/net/Proxy"
	.zero	61
	.zero	3

	/* #197 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554858
	/* java_name */
	.ascii	"java/net/ProxySelector"
	.zero	53
	.zero	3

	/* #198 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554860
	/* java_name */
	.ascii	"java/net/SocketAddress"
	.zero	53
	.zero	3

	/* #199 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554862
	/* java_name */
	.ascii	"java/net/URI"
	.zero	63
	.zero	3

	/* #200 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554877
	/* java_name */
	.ascii	"java/nio/Buffer"
	.zero	60
	.zero	3

	/* #201 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554879
	/* java_name */
	.ascii	"java/nio/ByteBuffer"
	.zero	56
	.zero	3

	/* #202 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/nio/channels/ByteChannel"
	.zero	46
	.zero	3

	/* #203 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/nio/channels/Channel"
	.zero	50
	.zero	3

	/* #204 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554881
	/* java_name */
	.ascii	"java/nio/channels/FileChannel"
	.zero	46
	.zero	3

	/* #205 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/nio/channels/GatheringByteChannel"
	.zero	37
	.zero	3

	/* #206 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/nio/channels/InterruptibleChannel"
	.zero	37
	.zero	3

	/* #207 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/nio/channels/ReadableByteChannel"
	.zero	38
	.zero	3

	/* #208 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/nio/channels/ScatteringByteChannel"
	.zero	36
	.zero	3

	/* #209 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/nio/channels/WritableByteChannel"
	.zero	38
	.zero	3

	/* #210 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554897
	/* java_name */
	.ascii	"java/nio/channels/spi/AbstractInterruptibleChannel"
	.zero	25
	.zero	3

	/* #211 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554865
	/* java_name */
	.ascii	"java/security/KeyStore"
	.zero	53
	.zero	3

	/* #212 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/security/KeyStore$LoadStoreParameter"
	.zero	34
	.zero	3

	/* #213 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/security/KeyStore$ProtectionParameter"
	.zero	33
	.zero	3

	/* #214 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554870
	/* java_name */
	.ascii	"java/security/cert/Certificate"
	.zero	45
	.zero	3

	/* #215 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554872
	/* java_name */
	.ascii	"java/security/cert/CertificateFactory"
	.zero	38
	.zero	3

	/* #216 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554875
	/* java_name */
	.ascii	"java/security/cert/X509Certificate"
	.zero	41
	.zero	3

	/* #217 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/security/cert/X509Extension"
	.zero	43
	.zero	3

	/* #218 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554824
	/* java_name */
	.ascii	"java/util/ArrayList"
	.zero	56
	.zero	3

	/* #219 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554813
	/* java_name */
	.ascii	"java/util/Collection"
	.zero	55
	.zero	3

	/* #220 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554815
	/* java_name */
	.ascii	"java/util/HashMap"
	.zero	58
	.zero	3

	/* #221 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554833
	/* java_name */
	.ascii	"java/util/HashSet"
	.zero	58
	.zero	3

	/* #222 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"java/util/Iterator"
	.zero	57
	.zero	3

	/* #223 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"javax/net/ssl/TrustManager"
	.zero	49
	.zero	3

	/* #224 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554535
	/* java_name */
	.ascii	"javax/net/ssl/TrustManagerFactory"
	.zero	42
	.zero	3

	/* #225 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"javax/net/ssl/X509TrustManager"
	.zero	45
	.zero	3

	/* #226 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554985
	/* java_name */
	.ascii	"mono/android/TypeManager"
	.zero	51
	.zero	3

	/* #227 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554770
	/* java_name */
	.ascii	"mono/android/content/DialogInterface_OnClickListenerImplementor"
	.zero	12
	.zero	3

	/* #228 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554774
	/* java_name */
	.ascii	"mono/android/content/DialogInterface_OnMultiChoiceClickListenerImplementor"
	.zero	1
	.zero	3

	/* #229 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554809
	/* java_name */
	.ascii	"mono/android/runtime/InputStreamAdapter"
	.zero	36
	.zero	3

	/* #230 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	0
	/* java_name */
	.ascii	"mono/android/runtime/JavaArray"
	.zero	45
	.zero	3

	/* #231 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554830
	/* java_name */
	.ascii	"mono/android/runtime/JavaObject"
	.zero	44
	.zero	3

	/* #232 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554848
	/* java_name */
	.ascii	"mono/android/runtime/OutputStreamAdapter"
	.zero	35
	.zero	3

	/* #233 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554621
	/* java_name */
	.ascii	"mono/android/view/View_OnClickListenerImplementor"
	.zero	26
	.zero	3

	/* #234 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554627
	/* java_name */
	.ascii	"mono/android/view/View_OnKeyListenerImplementor"
	.zero	28
	.zero	3

	/* #235 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554551
	/* java_name */
	.ascii	"mono/android/widget/AdapterView_OnItemClickListenerImplementor"
	.zero	13
	.zero	3

	/* #236 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554571
	/* java_name */
	.ascii	"mono/android/widget/CompoundButton_OnCheckedChangeListenerImplementor"
	.zero	6
	.zero	3

	/* #237 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554580
	/* java_name */
	.ascii	"mono/android/widget/ExpandableListView_OnChildClickListenerImplementor"
	.zero	5
	.zero	3

	/* #238 */
	/* module_index */
	.word	1
	/* type_token_id */
	.word	33554916
	/* java_name */
	.ascii	"mono/java/lang/RunnableImplementor"
	.zero	41
	.zero	3

	.size	map_java, 20554
/* Java to managed map: END */


/* java_name_width: START */
	.section	.rodata.java_name_width,"a",@progbits
	.type	java_name_width, @object
	.p2align	2
	.global	java_name_width
java_name_width:
	.size	java_name_width, 4
	.word	78
/* java_name_width: END */
