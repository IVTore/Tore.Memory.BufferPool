/*————————————————————————————————————————————————————————————————————————————
    ————————————————————————————————————————————————————————————————————————
    |   BufferPool : A C# Threadsafe ByteArray buffer pool class library   |
    ————————————————————————————————————————————————————————————————————————

© Copyright 2023 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Release             : 202304181000
Version             : 1.0.0.
License             : MIT.

History             :
202304181000: IVT   : First implementation.
————————————————————————————————————————————————————————————————————————————*/

using System;
using System.Collections.Generic;

namespace Tore.Memory {

    /**——————————————————————————————————————————————————————————————————————————— 
        CLASS:  BufferPool                                              <summary>
        USAGE:                                                          <br/>
            A Threadsafe ByteArray Buffer pool class.                   <br/>
            Allows a buffer pool up to 2GB.                             <para/>
            Construction:                                               <code>
            BufferPool pool;                                            <br/>
            pool = new (poolCapacity, sizePerBuffer, preAllocate);      </code>
            To get a buffer                                             <code>
            ByteArray myBuffer = pool.GetBuffer();                      </code>
            To return a buffer                                          <code>
            pool.PutBuffer(myBuffer);                                   </code>
                                                                        </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class BufferPool {

        // Constraints.
        private const long GIGABYTE = 1024 * 1024 * 1024; // 1 GB
        private const int MIN_BUFFER_SIZE = 64;
        private const int MIN_BUFFER_COUNT = 16;
        private const int MAX_RAM_GB = 2;

        private object poolLock = new();
        private Stack<byte[]> pool;
        private bool dead = false;
        
        /**———————————————————————————————————————————————————————————————————————————
          PROP: capacity: int.                                              <summary>
          GET : Gets the capacity, the limit of the pool.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int capacity { get; }

        /**———————————————————————————————————————————————————————————————————————————
          PROP: bufferSize: int.                                            <summary>
           GET: Gets the size per the buffers in the pool.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int bufferSize { get; }

        /**———————————————————————————————————————————————————————————————————————————
          PROP: count: int.                                                 <summary>
           GET: Gets the number of available buffers in the pool.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int count => pool.Count;

        #region Constructor and Destructor methods.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————————————————————————
            | Constructor and Destructor methods.  |
            ————————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**——————————————————————————————————————————————————————————————————————————
          CTOR: BufferPool.                                                 <summary>
          TASK:                                                             <br/>
            Constructs a thread safe ByteArray buffer pool.                 <para/>
          ARGS:                                                             <br/>
            poolCapacity  : int  : The maximum number of buffers allowed.   <br/>
            sizePerBuffer : int  : Number of bytes per ByteArray.           <br/>
            preAllocate   : bool : if true buffers are populated.:DEF:false.<para/>
          INFO:                                                             <br/>
            Constraints:                                                    <br/>
            MIN_BUFFER_COUNT: poolCapacity must be greater than 16.         <br/>
            MIN_BUFFER_SIZE : sizePerBuffer must be greater than 64.        <br/>
            MAX_RAM_GB       : poolCapacity * sizePerBuffer must be less 
                               than or equal to 2GB.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public BufferPool(int poolCapacity, int sizePerBuffer, bool preAllocate = false) {
            if (sizePerBuffer < MIN_BUFFER_SIZE)
                throw new ArgumentOutOfRangeException(nameof(sizePerBuffer) + " < " + MIN_BUFFER_SIZE.ToString());
            if (poolCapacity < MIN_BUFFER_COUNT)
                throw new ArgumentOutOfRangeException(nameof(poolCapacity) + " < " + MIN_BUFFER_COUNT.ToString());
            if (((long)poolCapacity * sizePerBuffer) > (MAX_RAM_GB * GIGABYTE))
                throw new ArgumentException("Pool > " + MAX_RAM_GB.ToString() + "GB!");
            capacity = poolCapacity;
            bufferSize = sizePerBuffer;
            pool = new(capacity);
            if (!preAllocate)
                return;
            for(var i = 0; i < capacity; i++)
                pool.Push(new byte[bufferSize]);
        }

        /**——————————————————————————————————————————————————————————————————————————
          DTOR: ~BufferPool.                                                <summary>
          TASK:                                                             <br/>
            Destructs the thread safe ByteArray buffer pool.                <para/>
          INFO:                                                             <br/>
            No disposable magic needed... Simple.                           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        ~BufferPool() {
            if (dead)
                return;
            dead = true;
            lock(poolLock)
                pool.Clear();
        }
        #endregion

        #region Buffer acquire - release methods.
        /*————————————————————————————————————————————————————————————————————————————
            ———————————————————————————————————————
            |  Buffer acquire - release methods.  |
            ———————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**——————————————————————————————————————————————————————————————————————————
          FUNC: PutBuffer.                                                  <summary>
          TASK:                                                             <br/>
            Returns a buffer to the pool.                                   <para/>
          ARGS:                                                             <br/>
            buffer : byte[] : The buffer to return.                         <para/>
          INFO:                                                             <br/>
            If the pool is full or the buffer is null or does not have      <br/>
            the correct size, it is discarded.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public void PutBuffer(byte[] buffer) {
            if (pool.Count >= capacity || buffer == null || buffer.Length != bufferSize)
                return;
            Array.Clear(buffer);
            lock(poolLock) { 
                if (pool.Count < capacity)  // Check again, just in case.
                    pool.Push(buffer);
            }
        }
        /**——————————————————————————————————————————————————————————————————————————
          FUNC: GetBuffer.                                                  <summary>
          TASK:                                                             <br/>
            Gets a buffer from the pool.                                    <para/>
          INFO:                                                             <br/>
            If pool has a buffer, returns it, otherwise creates a new one.  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public byte[] GetBuffer(){
            if (pool.Count == 0) 
                return new byte[bufferSize];
            lock(poolLock)
                return pool.Pop(); 
        }

        #endregion
    }
}
