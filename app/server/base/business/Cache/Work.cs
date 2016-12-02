#region Copyright

//==============================================================================
//  File Name   :   Work.cs
//
//  Copyright (C) 2011 E2 Technologies. All rights reserved.
//
//  Distributable under e2 technologies code license.
//  See terms of license at www.e2.org.cn
//
//==============================================================================

//==============================================================================
// <fileinformation>
//   <summary>
//      This file is a part of business project.
//   </summary>
//   <author name="Zhang Ling" mail="tox@e2.org.cn"/>
//   <seealso ref=""/>
// </fileinformation>
//
// <history>
//   <record date="2011-06-26 19:22:00" author="Zhang Ling" revision="1.00.000">
//		First version of Work.
//   </record>
// </history>
//==============================================================================

#endregion

namespace App.Web.Business.Cache
{
    using System;
    using System.Security.Cryptography;
    using App.Web.Business.Utils;

    /// <summary>
    ///  Summary of Work.
    /// </summary>
    /// <remarks>
    /// FIELD           | PURPOSE                                       | Updated when...               | Size (Bytes)
    /// --------------------------------------------------------------------------------------------------------------
    /// Version	        | Block version number                          | You upgrade the software      |   4
    /// Previous hash	| Hash of the previous block	                | A new block comes in	        |   32
    /// Merkle root	    | 256-bit hash based on all of the transactions	| A transaction is accepted	    |   32
    /// Timestamp	    | Current timestamp	                            | Every few seconds	            |   4
    /// "Bits"	        | Current target in compact format	            | The difficulty is adjusted	|   4
    /// Nonce	        | 32-bit number (starts at 0)	                | A hash is tried (increments)	|   4
    /// 
    /// Bitcoin uses: SHA256(SHA256(Block_Header))
    /// </remarks>
    public class Work
    {
        static readonly SHA256Managed encoder = new SHA256Managed();

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="Work"/> class.
        /// </summary>
        public Work(byte[] data)
        {
            //Remember to use little-endian.
            var decodedData = SwapInt32(data, 1, 8);
            decodedData = SwapInt32(decodedData, 9, 8);

            //decode the block hash
            Version = BitConverter.ToInt32(decodedData, 0);
            PreviousHash = BitConverter.ToString(decodedData, 4, 32).Replace("-", string.Empty);
            MerkleRoot = BitConverter.ToString(decodedData, 36, 32).Replace("-", string.Empty);
            TimestampSeconds = BitConverter.ToInt32(decodedData, 68);
            Timestamp = UnixTime.ConvertFromUnixTimestamp(TimestampSeconds);
            Target = BitConverter.ToInt32(decodedData, 72);
            Nonce = BitConverter.ToInt32(decodedData, 76);

            //for delay hash computing the first 80 bytes
            UnderlyingData = new byte[80];
            Array.Copy(data, 0, UnderlyingData, 0, 80);

        }

        public static Work Parse(string data)
        {
            if (data.Length != 256)
                return null;

            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    var bin = HexEncoding.Decode(data.Substring(0, 160));
                    if (bin.Length == 80)
                    {
                        return new Work(bin);
                    }
                }

                //sorry, we do not support big-endian system
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Properties

        public int Version { get; private set; }
        public string PreviousHash { get; private set; }
        public string MerkleRoot { get; private set; }
        public int TimestampSeconds { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int Target { get; private set; }
        public int Nonce { get; set; }

        public string Hash
        {
            get
            {
                if ( _hash == null && UnderlyingData != null )
                {
                    this.ComputeHash();
                }
                return _hash;
            }
        }
        public byte[] HashData
        {
            get
            {
                if (_hashData == null && UnderlyingData != null)
                {
                    this.ComputeHash();
                }
                return _hashData;
            }
        }
        public int HashScore
        {
            get 
            {
                if (_hash == null && UnderlyingData != null)
                {
                    this.ComputeHash();
                }
                return _hashScore;
            }
        }
        #endregion

        #region Fields

        private readonly byte[] UnderlyingData;
        private byte[] _hashData;
        private string _hash;
        private int _hashScore;

        #endregion

        #region Helper

        private void ComputeHash()
        {
            lock (UnderlyingData)
            {
                //sha256(sha256(data)) 
                var hashDataSwaped = SwapByInt32(UnderlyingData);
                _hashData = encoder.ComputeHash(encoder.ComputeHash(hashDataSwaped));

                //swap the low and high for print
                var displayHash = SwapInt256(_hashData);
                _hash = BitConverter.ToString(displayHash).Replace("-", string.Empty);

                for (var idx = 7; idx < 16; idx++)
                {
                    if (_hash[idx] == '0')
                    {
                        _hashScore = idx;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static byte[] SwapInt256(byte[] source)
        {
            var tmp = new byte[source.Length];
            var pos = 0;
            source.CopyTo(tmp, 0);
            for (var offset = source.Length - 1; offset >= 0; --offset)
            {
                tmp[pos] = source[offset];
                ++pos;
            }
            return tmp;
        }

        private static byte[] SwapByInt32(byte[] source)
        {
            var tmp = new byte[source.Length];
            var pos = 0;
            source.CopyTo(tmp, 0);

            var blocks = source.Length / 4;
            for (var b = 0; b < blocks; b++)
            {
                for (var offset = 0; offset < 4; ++offset)
                {
                    tmp[pos] = source[b * 4 + (3 - offset)];
                    ++pos;
                }
            }
            return tmp;
        }

        private static byte[] SwapInt32(byte[] source, int start, int length)
        {
            //int contains 4 bytes
            var tmp = new byte[source.Length];
            var pos = start * 4;

            source.CopyTo(tmp, 0);

            for (var blk = start + length - 1; blk >= start; --blk)
            {
                for (var offset = 0; offset < 4; ++offset)
                {
                    tmp[pos] = source[blk * 4 + offset];
                    ++pos;
                }
            }

            return tmp;
        } 
        #endregion
    }
}