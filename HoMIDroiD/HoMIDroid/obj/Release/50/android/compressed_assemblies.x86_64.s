	.file	"compressed_assemblies.x86_64.x86_64.s"
	.include	"compressed_assemblies.x86_64-data.inc"

	.section	.data.compressed_assembly_descriptors,"aw",@progbits
	.type	.L.compressed_assembly_descriptors, @object
	.p2align	4
.L.compressed_assembly_descriptors:
	/* 0: HoMIDroid.dll */
	/* uncompressed_file_size */
	.long	374272
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_0

	/* 1: I18N.West.dll */
	/* uncompressed_file_size */
	.long	82832
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_1

	/* 2: I18N.dll */
	/* uncompressed_file_size */
	.long	49552
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_2

	/* 3: Java.Interop.dll */
	/* uncompressed_file_size */
	.long	162304
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_3

	/* 4: Mono.Android.dll */
	/* uncompressed_file_size */
	.long	940544
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_4

	/* 5: Mono.Security.dll */
	/* uncompressed_file_size */
	.long	120832
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_5

	/* 6: System.Core.dll */
	/* uncompressed_file_size */
	.long	318464
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_6

	/* 7: System.Numerics.dll */
	/* uncompressed_file_size */
	.long	25600
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_7

	/* 8: System.Web.Services.dll */
	/* uncompressed_file_size */
	.long	77312
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_8

	/* 9: System.Xml.dll */
	/* uncompressed_file_size */
	.long	890880
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_9

	/* 10: System.dll */
	/* uncompressed_file_size */
	.long	667648
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_10

	/* 11: mscorlib.dll */
	/* uncompressed_file_size */
	.long	1911808
	/* loaded */
	.byte	0
	/* data */
	.zero	3
	.quad	compressed_assembly_data_11

	.size	.L.compressed_assembly_descriptors, 192
	.section	.data.compressed_assemblies,"aw",@progbits
	.type	compressed_assemblies, @object
	.p2align	3
	.global	compressed_assemblies
compressed_assemblies:
	/* count */
	.long	12
	/* descriptors */
	.zero	4
	.quad	.L.compressed_assembly_descriptors
	.size	compressed_assemblies, 16
