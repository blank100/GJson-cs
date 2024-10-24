using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gal.Core
{
    /// <summary>
    /// 栈上的 Stack
    /// </summary>
    /// <para>author gouanlin</para>
    public ref struct RefStack<T>
    {
        private T[] m_Buffer;
        private Span<T> m_Span;
        private int m_Position;

        public int count => m_Position;

        public RefStack(Span<T> buffer) {
            m_Buffer = default;
            m_Span = buffer;
            m_Position = 0;
        }

        public RefStack(int capacity) {
            Debug.Assert(capacity >= 0, $"The parameter {nameof(capacity)} cannot be negative");

            m_Buffer = ArrayPool<T>.Shared.Rent(capacity);
            m_Span = m_Buffer;
            m_Position = 0;
        }

        /// <summary>
        /// 在当前位置写入一个元素,并将 position 向后移动1位
        /// </summary>
        /// <param name="element"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T element) {
            if (m_Position + 1 > m_Span.Length) GrowBuffer(1);
            m_Span[m_Position++] = element;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop() => m_Span[--m_Position];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek() => m_Span[m_Position - 1];

        /// <summary>
        /// 增长 buffer
        /// </summary>
        /// <param name="growSize">增长的长度</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GrowBuffer(int growSize) {
            var len = m_Span.Length;
            GenerateBuffer(checked(len + Math.Max(growSize, len)));
        }

        /// <summary>
        /// 生成新的 buffer ,并回收原 buffer
        /// </summary>
        /// <param name="size"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GenerateBuffer(int size) {
            var buffer = ArrayPool<T>.Shared.Rent(size);
            m_Span[..m_Position].CopyTo(buffer);
            if (m_Buffer != null) ArrayPool<T>.Shared.Return(m_Buffer, !typeof(T).IsValueType);
            m_Span = m_Buffer = buffer;
        }

        /// <summary>
        /// 清理
        /// <para>不会真实的清理所有元素,只是将 position 和 length 置为 0 </para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => m_Position = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() {
            var buffer = m_Buffer;
            this = default;
            if (buffer != null) ArrayPool<T>.Shared.Return(buffer, !typeof(T).IsValueType);
        }
    }
}