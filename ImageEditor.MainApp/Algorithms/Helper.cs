using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor.MainApp.Algorithms
{
    class Helper
    {
        private static int maxAllocatedProcessors = Environment.ProcessorCount;
        public static int MaxAllocatedProcessors
        {
            get => maxAllocatedProcessors;
            set
            {
                if (value > 0 && value <= Environment.ProcessorCount)
                {
                    maxAllocatedProcessors = value;
                }
            }
        }

        private static Convolution.EdgeHandling edgeHandling = Convolution.EdgeHandling.Extend;
        public static Convolution.EdgeHandling EdgeHandling
        {
            get => edgeHandling;
            set => edgeHandling = value;
        }

        private static Resample.Type resamplingType = Resample.Type.Nearest;
        public static Resample.Type ResamplingType
        {
            get => resamplingType;
            set => resamplingType = value;
        }
    }
}
