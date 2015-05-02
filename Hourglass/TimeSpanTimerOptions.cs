﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanTimerOptions.cs" company="Chris Dziemborowicz">
//   Copyright (c) Chris Dziemborowicz. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hourglass
{
    using System;

    /// <summary>
    /// Configuration data for a <see cref="TimeSpanTimer"/>.
    /// </summary>
    public class TimeSpanTimerOptions : TimerOptions
    {
        #region Private Members

        /// <summary>
        /// A value indicating whether to loop the timer continuously.
        /// </summary>
        private bool loopTimer;

        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanTimerOptions"/> class.
        /// </summary>
        public TimeSpanTimerOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanTimerOptions"/> class from a <see
        /// cref="TimeSpanTimerOptions"/>.
        /// </summary>
        /// <param name="options">A <see cref="TimeSpanTimerOptions"/>.</param>
        public TimeSpanTimerOptions(TimeSpanTimerOptions options)
            : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.loopTimer = options.LoopTimer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanTimerOptions"/> class from a <see
        /// cref="TimeSpanTimerOptionsInfo"/>.
        /// </summary>
        /// <param name="optionsInfo">A <see cref="TimeSpanTimerOptionsInfo"/>.</param>
        public TimeSpanTimerOptions(TimeSpanTimerOptionsInfo optionsInfo)
            : base(optionsInfo)
        {
            if (optionsInfo == null)
            {
                throw new ArgumentNullException("optionsInfo");
            }

            this.loopTimer = optionsInfo.LoopTimer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to loop the timer continuously.
        /// </summary>
        public bool LoopTimer
        {
            get
            {
                return this.loopTimer;
            }

            set
            {
                this.ThrowIfFrozen();

                if (this.loopTimer == value)
                {
                    return;
                }

                this.loopTimer = value;
                this.OnPropertyChanged("LoopTimer");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">An <see cref="object"/>.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object, or <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            if (!base.Equals(obj))
            {
                return false;
            }

            TimeSpanTimerOptions options = (TimeSpanTimerOptions)obj;
            return object.Equals(this.loopTimer, options.loopTimer);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            this.ThrowIfNotFrozen();

            int hashCode = 17;
            // ReSharper disable NonReadonlyFieldInGetHashCode
            hashCode = (31 * hashCode) + base.GetHashCode();
            hashCode = (31 * hashCode) + this.loopTimer.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
            return hashCode;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns a new <see cref="TimerOptionsInfo"/> of the correct type for this class.
        /// </summary>
        /// <returns>A new <see cref="TimerOptionsInfo"/>.</returns>
        protected override TimerOptionsInfo GetNewTimerOptionsInfo()
        {
            return new TimeSpanTimerOptionsInfo();
        }

        /// <summary>
        /// Sets the properties on a <see cref="TimerOptionsInfo"/> from the values in this class.
        /// </summary>
        /// <param name="timerOptionsInfo">A <see cref="TimerOptionsInfo"/>.</param>
        protected override void SetTimerOptionsInfo(TimerOptionsInfo timerOptionsInfo)
        {
            base.SetTimerOptionsInfo(timerOptionsInfo);

            TimeSpanTimerOptionsInfo info = (TimeSpanTimerOptionsInfo)timerOptionsInfo;
            info.LoopTimer = this.loopTimer;
        }

        #endregion
    }
}
