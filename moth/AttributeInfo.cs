using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moth
{
    class AttributeInfo
    {
        public float Value = 0.0f;
        public float Delta = 0.0f;
        public float Min = 0.0f;
        public float Max = 0.0f;
        public float Acceleration = 1.0f;
        public DeltaBehavior Behavior = DeltaBehavior.Stop;
        public bool OnUpdate = false;

        public AttributeInfo Clone()
        {
            return (AttributeInfo)this.MemberwiseClone();
        }

        public void UpdateDeltas(float multiplier = 1)
        {
            this.Value += this.Delta * multiplier;
            this.Delta *= this.Acceleration;

            if (this.Max == this.Min)
            {
                return;
            }

            switch (this.Behavior)
            {
                case DeltaBehavior.Wrap:
                    while (this.Value < this.Min)
                    {
                        this.Value += (this.Max - this.Min);
                    }
                    while (this.Value > this.Max)
                    {
                        this.Value -= (this.Max - this.Min);
                    }
                    break;
                case DeltaBehavior.Bounce:
                    if (this.Value < this.Min)
                    {
                        float diff = this.Min - this.Value;
                        diff %= this.Max - this.Min;
                        this.Value = this.Min + diff;
                        this.Delta *= -1;
                    }
                    else if (this.Value > this.Max)
                    {
                        float diff = this.Value - this.Max;
                        diff %= this.Max - this.Min;
                        this.Value = this.Max - diff;
                        this.Delta *= -1;
                    }
                    break;
                case DeltaBehavior.Stop:
                    if (this.Value < this.Min)
                    {
                        this.Value = this.Min;
                        this.Delta = 0;
                    }
                    else if (this.Value > this.Max)
                    {
                        this.Value = this.Max;
                        this.Delta = 0;
                    }
                    break;
            }
        }
    }
}
