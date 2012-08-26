﻿using System;
using System.IO;

namespace dot10.PE {
	/// <summary>
	/// Represents the IMAGE_NT_HEADERS PE section
	/// </summary>
	class ImageNTHeaders : FileSection {
		uint signature;
		ImageFileHeader imageFileHeader;
		IImageOptionalHeader imageOptionalHeader;

		/// <summary>
		/// Returns the IMAGE_NT_HEADERS.Signature field
		/// </summary>
		public uint Signature {
			get { return signature; }
		}

		/// <summary>
		/// Returns the IMAGE_NT_HEADERS.FileHeader field
		/// </summary>
		public ImageFileHeader FileHeader {
			get { return imageFileHeader; }
		}

		/// <summary>
		/// Returns the IMAGE_NT_HEADERS.OptionalHeader field
		/// </summary>
		public IImageOptionalHeader OptionalHeader {
			get { return imageOptionalHeader; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reader">PE file reader pointing to the start of this section</param>
		/// <param name="verify">Verify section</param>
		/// <exception cref="BadImageFormatException">Thrown if verification fails</exception>
		public ImageNTHeaders(BinaryReader reader, bool verify) {
			SetStartOffset(reader);
			this.signature = reader.ReadUInt32();
			if (verify && this.signature != 0x4550)
				throw new BadImageFormatException("Invalid NT headers signature");
			this.imageFileHeader = new ImageFileHeader(reader, verify);
			this.imageOptionalHeader = CreateImageOptionalHeader(reader, verify);
			//TODO: set end offset
		}

		/// <summary>
		/// Creates an IImageOptionalHeader
		/// </summary>
		/// <param name="reader">PE file reader pointing to the start of the optional header</param>
		/// <param name="verify">Verify section</param>
		/// <returns>The created IImageOptionalHeader</returns>
		/// <exception cref="BadImageFormatException">Thrown if verification fails</exception>
		IImageOptionalHeader CreateImageOptionalHeader(BinaryReader reader, bool verify) {
			ushort magic = reader.ReadUInt16();
			reader.BaseStream.Position -= 2;
			switch (magic) {
			case 0x010B: return new ImageOptionalHeader32(reader, imageFileHeader.SizeOfOptionalHeader, verify);
			case 0x020B: return new ImageOptionalHeader32(reader, imageFileHeader.SizeOfOptionalHeader, verify);
			default: throw new BadImageFormatException("Invalid optional header magic");
			}
		}
	}
}
