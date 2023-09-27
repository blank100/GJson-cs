﻿// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY gouanlin. DO NOT CHANGE IT.
// </auto-generated>

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gal.Core
{
    public static class WriterByteEx {
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteInt8(this IWriter<byte> self, sbyte value){ 
            self.Write((byte)value);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteUInt8(this IWriter<byte> self, byte value){
            self.Write(value);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteBoolean(this IWriter<byte> self, bool value){
            self.Write(value ? (byte)1 : (byte)0);
                        return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteUInt16(this IWriter<byte> self, UInt16 value) {
			var span = self.GetSpan(2);
            span[1] = (byte)(value >> 8);
			span[0] = (byte)value;
			self.Advance(2);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteInt16(this IWriter<byte> self, Int16 value) {
			var span = self.GetSpan(2);
            span[1] = (byte)(value >> 8);
			span[0] = (byte)value;
			self.Advance(2);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteInt32(this IWriter<byte> self, Int32 value) {
			Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(self.GetSpan(4)), !BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
            self.Advance(4);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteUInt32(this IWriter<byte> self, UInt32 value) {
			Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(self.GetSpan(4)), !BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
            self.Advance(4);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteInt64(this IWriter<byte> self, Int64 value) {
			Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(self.GetSpan(8)), !BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
            self.Advance(8);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteUInt64(this IWriter<byte> self, UInt64 value) {
			Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(self.GetSpan(8)), BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
            self.Advance(8);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IWriter<byte> WriteFloat(this IWriter<byte> self, float value) => self.WriteInt32(*(int*) &value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IWriter<byte> WriteDouble(this IWriter<byte> self, double value) => self.WriteInt64(*(long*) &value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteVarUInt32(this IWriter<byte> self, uint value) {
			while (value >= 0b10000000) {
				self.Write((byte)(value | 0b10000000));
				value >>= 7;
			}
			self.Write((byte)value);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteVarInt32(this IWriter<byte> self, int value) => self.WriteVarUInt32(ZigZagUtils.EncodeZigZag32(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteVarUInt64(this IWriter<byte> self, ulong value) {
			while (value >= 0b10000000) {
				self.Write((byte)(value | 0b10000000));
				value >>= 7;
			}
			self.Write((byte)value);
            return self;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IWriter<byte> WriteVarInt64(this IWriter<byte> self, long value){ 
            self.WriteVarUInt64(ZigZagUtils.EncodeZigZag64(value));
            return self;
        }

        public static unsafe IWriter<byte> WriteUtf8(this IWriter<byte> self, string value, int bytesCount) {
			if (string.IsNullOrEmpty(value)) {
                return self;
			}
			fixed(char* source = value){
			    fixed (byte* target = self.GetSpan(bytesCount)) {
				    System.Text.Encoding.UTF8.GetBytes(source, value.Length, target, bytesCount);
			    }
            }
			self.Advance(bytesCount);
            return self;
        }

        public static unsafe IWriter<byte> WriteUtf8(this IWriter<byte> self, string value) {
			if (string.IsNullOrEmpty(value)) {
				self.WriteInt16(0);
                return self;
			}
			
			var bytesCount   = System.Text.Encoding.UTF8.GetByteCount(value);
			self.WriteInt16((short)bytesCount);

			fixed(char* source = value){
			    fixed (byte* target = self.GetSpan(bytesCount)) {
				    System.Text.Encoding.UTF8.GetBytes(source, value.Length, target, bytesCount);
			    }
            }
			self.Advance(bytesCount);
            return self;
        }
    }
}