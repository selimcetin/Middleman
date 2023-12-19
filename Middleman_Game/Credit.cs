using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_Game
{
    public class Credit
    {
        private int sum;
        private float percentage;
        private float repayment;
        private int duration;
        private int dayDue;

        public int Sum
        {
            get => sum;
            set => sum = value;
        }

        public float Percentage
        {
            get => percentage;
            set => percentage = value;
        }

        public float Repayment
        {
            get => repayment;
            set => repayment = value;
        }

        public int Duration
        {
            get => duration;
            set => duration = value;
        }

        public int DayDue
        {
            get => dayDue;
            set => dayDue = value;
        }

        public Credit(int sum, float percentage)
        {
            this.sum = sum;
            this.percentage = percentage;
            Repayment = sum * (1 + percentage / 100);
            Duration = 7;
            DayDue = Duration;
        }
    }
}