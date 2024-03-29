﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Logging.Counters
{
    public class Counter: Stats
    {
        public Counter()
        {
            DefaultCountReader = () => _count;
            CountReader = DefaultCountReader;
        }

        protected Func<ulong> DefaultCountReader { get; }

        public override object Value
        {
            get
            {
                return Count;
            }
            set
            {
                Count = (ulong)value;
            }
        }

        ulong _count;
        public ulong Count
        {
            get
            {
                return CountReader();
            }
            set
            {
                _count = value;
            }
        }

        public Func<ulong> CountReader { get; set; }

        public Counter Increment()
        {
            ++_count;
            if(CountReader != DefaultCountReader)
            {
                Log.Trace("Increment called on counter ({0}) with custom CountReader; this may not behave as expected.", Name);
            }
            return this;
        }

        public Counter Decrement()
        {
            --_count;
            if (CountReader != DefaultCountReader)
            {
                Log.Trace("Decrement called on counter ({0}) with custom CountReader; this may not behave as expected.", Name);
            }
            return this;
        }

        public Counter Diff(Counter counter)
        {
            return new Counter() { Name = Name, Value = Count - counter.Count };
        }
    }
}
