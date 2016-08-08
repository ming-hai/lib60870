/*
 *  BitString32.cs
 *
 *  Copyright 2016 MZ Automation GmbH
 *
 *  This file is part of lib60870.NET
 *
 *  lib60870.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  lib60870.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with lib60870.NET.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  See COPYING file for the complete license text.
 */

using System;

namespace lib60870
{
	
	public class Bitstring32 : InformationObject
	{

		private UInt32 value;

		public UInt32 Value {
			get {
				return this.value;
			}
		}

		private QualityDescriptor quality;

		public QualityDescriptor Quality {
			get {
				return this.quality;
			}
		}

		public Bitstring32 (ConnectionParameters parameters, byte[] msg, int startIndex) :
			base(parameters, msg, startIndex)
		{
			startIndex += parameters.SizeOfIOA; /* skip IOA */

			value = msg [startIndex++];
			value += ((uint)msg [startIndex++] * 0x100);
			value += ((uint)msg [startIndex++] * 0x10000);
			value += ((uint)msg [startIndex++] * 0x1000000);

			quality = new QualityDescriptor (msg[startIndex++]);

		}

		public override void Encode(Frame frame, ConnectionParameters parameters) {
			base.Encode(frame, parameters);

			frame.SetNextByte((byte) (value % 0x100));
			frame.SetNextByte((byte) ((value / 0x100) % 0x100));
			frame.SetNextByte((byte) ((value / 0x10000) % 0x100));
			frame.SetNextByte((byte) (value / 0x1000000));

			frame.SetNextByte (quality.EncodedValue);
		}
	}

	public class Bitstring32WithCP24Time2a : Bitstring32
	{

		private CP24Time2a timestamp;

		public CP24Time2a Timestamp {
			get {
				return this.timestamp;
			}
		}

		public Bitstring32WithCP24Time2a (ConnectionParameters parameters, byte[] msg, int startIndex) :
			base(parameters, msg, startIndex)
		{
			startIndex += parameters.SizeOfIOA + 5; /* skip IOA + value + quality */

			/* parse CP24Time2a (time stamp) */
			timestamp = new CP24Time2a (msg, startIndex);
		}

		public override void Encode(Frame frame, ConnectionParameters parameters) {
			base.Encode(frame, parameters);

			frame.AppendBytes (timestamp.GetEncodedValue ());
		}

	}

	public class Bitstring32WithCP56Time2a : Bitstring32
	{
		private CP56Time2a timestamp;

		public CP56Time2a Timestamp {
			get {
				return this.timestamp;
			}
		}

		public Bitstring32WithCP56Time2a (ConnectionParameters parameters, byte[] msg, int startIndex) :
			base(parameters, msg, startIndex)
		{
			startIndex += parameters.SizeOfIOA + 5; /* skip IOA + value + quality */

			/* parse CP56Time2a (time stamp) */
			timestamp = new CP56Time2a (msg, startIndex);
		}

		public override void Encode(Frame frame, ConnectionParameters parameters) {
			base.Encode(frame, parameters);

			frame.AppendBytes (timestamp.GetEncodedValue ());
		}

	}

}

