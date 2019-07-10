using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Compression
{
    /// <summary>
    /// 压缩文件实体类
    /// </summary>
#if DELUXEWORKSTEST
    public class ZipEntry
#else
    internal class ZipEntry
#endif
    {
        //static int KNOWN_SIZE = 1;
        //static int KNOWN_CSIZE = 2;
        private static int KNOWN_CRC = 4;
        private static int KNOWN_TIME = 8;
		//private static int KNOWN_EXTERN_ATTRIBUTES = 16;

        private ushort known = 0;                       // Bit flags made up of above bits

        #region private fields
        private Int32 size;
        private Int32 compressedSize;
        //private ulong compressedSize;
        private Int32 dosTime;			//Time represented as an Int
		//private Int16 nameLength;		//Length of the variable sized name
        //private byte[] crc;             //Array of 16 CRC bytes
        private uint crc;
        private string name;
        private int offset;

		//private int flags;                             // general purpose bit flags
        private byte[] extra = null;
		//private string comment = null;
		//private int zipFileIndex = -1;                 // used by ZipFile

		//private int externalFileAttributes = -1;     // contains external attributes (os dependant)

        #endregion

        #region constructor

        /// <summary>
        /// 用文件名创建一个压缩文件实体对象
        /// </summary>
        /// <param name="name">
        /// 文件名，可以包含路径。
        /// </param>
        public ZipEntry(string name)
        {
            Core.ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");
            this.DateTime = System.DateTime.Now;
            this.Name = name;
            this.size = 0;
            this.compressedSize = 0;
            //this.crc = new byte[16];
        }

        #endregion

        #region public property
        /// <summary>
        /// 表现为int型的操作时间
        /// </summary>
        public Int32 DosTime
        {
            get
            {
                return this.dosTime;
            }
            set
            {
                this.dosTime = value;
            }
        }

		//public Int16 NameLength
		//{
		//    get
		//    {
		//        return this.nameLength;
		//    }
		//    set
		//    {
		//        //Check if the value is greater than 16 bytes
		//        if ((UInt16)value > 0xffff)
		//            throw new ArgumentOutOfRangeException();
		//        this.nameLength = value;
		//    }
		//}

        /// <summary>
        /// 设置实体的最后修改时间
        /// </summary>
        public DateTime DateTime
        {
			//get
			//{
			//    int sec = 2 * (this.dosTime & 0x1f);
			//    int min = (this.dosTime >> 5) & 0x3f;
			//    int hrs = (this.dosTime >> 11) & 0x1f;
			//    int day = (this.dosTime >> 16) & 0x1f;
			//    int mon = ((this.dosTime >> 21) & 0xf);
			//    int year = ((this.dosTime >> 25) & 0x7f) + 1980; /* since 1900 */
			//    return new System.DateTime(year, mon, day, hrs, min, sec);
			//}
            set
            {
                DosTime = ((Int32)value.Year - 1980 & 0x7f) << 25 |
                          ((Int32)value.Month) << 21 |
                          ((Int32)value.Day) << 16 |
                          ((Int32)value.Hour) << 11 |
                          ((Int32)value.Minute) << 5 |
                          ((Int32)value.Second) >> 1;
            }
        }

        /// <summary>
        /// 获取或设置实体名称，路径用“/”分割。
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                //Check if the value is greater than 16 bytes or null
                if (value == null || value.Length > 0xffffL)
                    throw new ArgumentOutOfRangeException();

                if (value.Length != 0)
                {
                    this.name = value;
					//this.nameLength = (Int16)value.Length;
                }
            }
        }

        /// <summary>
        /// 获取或设置待压缩数据的大小。
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// 如果大小不在0..0xffffffffL之间则抛出异常
        /// </exception>
        /// <returns>
        /// 待压缩数据的大小 
        /// </returns>
        public Int32 Size
        {
            get
            {
                return this.size;
            }
            set
            {
                //Check if the value is greater than 32 bytes
                if ((UInt32)value > 0xffffffffL)
                    throw new ArgumentOutOfRangeException();

                this.size = (Int32)value;
            }
        }
        /// <summary>
        /// 获取或设置压缩数据的大小。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 如果大小不在0..0xffffffffL之间则抛出异常
        /// </exception>
        /// <returns>
        /// 压缩数据的大小 
        /// </returns>
        public int CompressedSize
        {
            get
            {
                return this.compressedSize;
            }
            set
            {
                //Check if the value is greater than 32 bytes
                if ((UInt32)value > 0xffffffffUL)
                    throw new ArgumentOutOfRangeException();

                this.compressedSize = (Int32)value;

            }
        }

        ///// <summary>
        ///// Gets/Sets the crc of the compressed data.
        ///// </summary>
        ///// <exception cref="System.ArgumentOutOfRangeException">
        ///// if crc is not in 16 byte array
        ///// </exception>
        ///// <returns>
        ///// the crc.
        ///// </returns>
        //public byte[] Crc
        //{
        //    get
        //    {
        //        return crc;
        //    }
        //    set
        //    {
        //        //Check if the Length of value array is greater than 16
        //        if (value.Length != crc.Length)
        //            throw new ArgumentOutOfRangeException();

        //        crc = value;
        //    }
        //}
        /// <summary>
        /// 获取或设置待压缩文件的crc
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 如果Crc不在0..0xffffffffL范围内则抛出异常
        /// </exception>
        /// <returns>
        /// crc的值，如果crc未知则为-1。
        /// </returns>
        public long Crc
        {
            get
            {
                return (this.known & ZipEntry.KNOWN_CRC) != 0 ? this.crc & 0xffffffffL : -1L;
            }
            set
            {
                if (((ulong)this.crc & 0xffffffff00000000L) != 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.crc = (uint)value;
				this.known |= (ushort)ZipEntry.KNOWN_CRC;
            }
        }

        /// <summary>
        /// 获取或设置central header的偏移量
        /// </summary>
        public int Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                if (((ulong)value & 0xFFFFFFFF00000000L) != 0)
                {
                    throw new ArgumentOutOfRangeException("Offset");
                }
                this.offset = value;
            }
        }

		///// <summary>
		///// Get/Set general purpose bit flag for entry
		///// </summary>
		///// <remarks>
		///// General purpose bit flag<br/>
		///// Bit 0: If set, indicates the file is encrypted<br/>
		///// Bit 1-2 Only used for compression type 6 Imploding, and 8, 9 deflating<br/>
		///// Imploding:<br/>
		///// Bit 1 if set indicates an 8K sliding dictionary was used.  If clear a 4k dictionary was used<br/>
		///// Bit 2 if set indicates 3 Shannon-Fanno trees were used to encode the sliding dictionary, 2 otherwise<br/>
		///// <br/>
		///// Deflating:<br/>
		/////   Bit 2    Bit 1<br/>
		/////     0        0       Normal compression was used<br/>
		/////     0        1       Maximum compression was used<br/>
		/////     1        0       Fast compression was used<br/>
		/////     1        1       Super fast compression was used<br/>
		///// <br/>
		///// Bit 3: If set, the fields crc-32, compressed size
		///// and uncompressed size are were not able to be written during zip file creation
		///// The correct values are held in a data descriptor immediately following the compressed data. <br/>
		///// Bit 4: Reserved for use by PKZIP for enhanced deflating<br/>
		///// Bit 5: If set indicates the file contains compressed patch data<br/>
		///// Bit 6: If set indicates strong encryption was used.<br/>
		///// Bit 7-15: Unused or reserved<br/>
		///// </remarks>
		//public int Flags
		//{
		//    get
		//    {
		//        return this.flags;
		//    }
		//    set
		//    {
		//        this.flags = value;
		//    }
		//}

		///// <summary>
		///// Gets/Sets the entry comment.
		///// </summary>
		///// <exception cref="System.ArgumentOutOfRangeException">
		///// If comment is longer than 0xffff.
		///// </exception>
		///// <returns>
		///// The comment or null if not set.
		///// </returns>
		//public string Comment
		//{
		//    //get
		//    //{
		//    //    return this.comment;
		//    //}
		//    set
		//    {
		//        // TODO: this test is strictly incorrect as the length is in characters
		//        // While the test is correct in that a comment of this length or greater 
		//        // is definitely invalid, shorter comments may also have an invalid length.
		//        if (value != null && value.Length > 0xffff)
		//        {
		//            throw new ArgumentOutOfRangeException();
		//        }
		//        this.comment = value;
		//    }
		//}

		///// <summary>
		///// Get/Set index of this entry in Zip file
		///// </summary>
		//public int ZipFileIndex
		//{
		//    get
		//    {
		//        return this.zipFileIndex;
		//    }
		//    set
		//    {
		//        this.zipFileIndex = value;
		//    }
		//}

		///// <summary>
		///// Get/Set external file attributes as an integer.
		///// The values of this are operating system dependant see
		///// <c>HostSystem</c> for details
		///// </summary>
		//public int ExternalFileAttributes
		//{
		//    //get
		//    //{
		//    //    if ((this.known & ZipEntry.KNOWN_EXTERN_ATTRIBUTES) == 0)
		//    //    {
		//    //        return -1;
		//    //    }
		//    //    else
		//    //    {
		//    //        return this.externalFileAttributes;
		//    //    }
		//    //}
		//    set
		//    {
		//        //this.externalFileAttributes = value;
		//        this.known |= (ushort)ZipEntry.KNOWN_EXTERN_ATTRIBUTES;
		//    }
		//}

        /// <summary>
        /// 设置超大数据
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 超大数据长度超过0xffff字节则抛出异常
        /// </exception>
        public byte[] ExtraData
        {
			//get
			//{
			//    return this.extra;
			//}
            set
            {
                if (value == null)
                {
                    this.extra = null;
                    return;
                }

                if (value.Length > 0xffff)
                {
                    throw new System.ArgumentOutOfRangeException();
                }

                this.extra = new byte[value.Length];
                Array.Copy(value, 0, this.extra, 0, value.Length);

                try
                {
                    int pos = 0;
                    while (pos < this.extra.Length)
                    {
                        int sig = (this.extra[pos++] & 0xff) | (this.extra[pos++] & 0xff) << 8;
                        int len = (this.extra[pos++] & 0xff) | (this.extra[pos++] & 0xff) << 8;

                        if (len < 0 || pos + len > this.extra.Length)
                        {
                            // This is still lenient but the extra data is corrupt
                            // TODO: drop the extra data? or somehow indicate to user 
                            // there is a problem...
                            break;
                        }

                        if (sig == 0x5455)
                        {
                            // extended time stamp, unix format by Rainer Prem <Rainer@Prem.de>
                            int flags = this.extra[pos];
                            // Can include other times but these are ignored.  Length of data should
                            // actually be 1 + 4 * no of bits in flags.
                            if ((flags & 1) != 0 && len >= 5)
                            {
                                int iTime = ((this.extra[pos + 1] & 0xff) |
                                    (this.extra[pos + 2] & 0xff) << 8 |
                                    (this.extra[pos + 3] & 0xff) << 16 |
                                    (this.extra[pos + 4] & 0xff) << 24);

                                DateTime = (new DateTime(1970, 1, 1, 0, 0, 0) + new TimeSpan(0, 0, 0, iTime, 0)).ToLocalTime();
								this.known |= (ushort)ZipEntry.KNOWN_TIME;
                            }
                        }
                        else if (sig == 0x0001)
                        {
                            // ZIP64 extended information extra field
                            // Of variable size depending on which fields in header are too small
                            // fields appear here if the corresponding local or central directory record field
                            // is set to 0xFFFF or 0xFFFFFFFF and the entry is in Zip64 format.
                            //
                            // Original Size          8 bytes
                            // Compressed size        8 bytes
                            // Relative header offset 8 bytes
                            // Disk start number      4 bytes
                        }
                        pos += len;
                    }
                }
                catch (Exception)
                {
                    /* be lenient */
                    return;
                }
            }
        }
        #endregion
    }
}
