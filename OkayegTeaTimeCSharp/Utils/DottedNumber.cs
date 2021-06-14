﻿namespace OkayegTeaTimeCSharp.Utils
{
    /// <summary>
    /// Creates a number in which every three digits are devided by a dot.<br/>
    /// For example: 1.465.564
    /// </summary>
    public struct DottedNumber
    {
        /// <summary>
        /// The number with the dots inserted.
        /// </summary>
        public string Number { get; }

        /// <summary>
        /// The original number passed to the constructor.
        /// </summary>
        public long OrigninalNumber { get; }

        /// <summary>
        /// The basic constructor of DottedNumber.
        /// </summary>
        /// <param name="number">A number of type long in which the dots will be inserted</param>
        public DottedNumber(long number)
        {
            string num = number.ToString();
            if (num.Length >= 4)
            {
                for (int i = num.Length - 3; i > 0; i -= 3)
                {
                    num = num.Insert(i, ".");
                }
            }
            Number = num;
            OrigninalNumber = number;
        }

        /// <summary>
        /// Override method of ToString() that returns the result.
        /// </summary>
        /// <returns>the result dotted number</returns>
        public override string ToString() => Number;
    }
}
