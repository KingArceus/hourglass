﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timer.cs" company="Chris Dziemborowicz">
//   Copyright (c) Chris Dziemborowicz. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hourglass.Timing
{
    using System;
    using System.Globalization;

    using Hourglass.Extensions;
    using Hourglass.Properties;
    using Hourglass.Serialization;

    /// <summary>
    /// A countdown timer.
    /// </summary>
    public class Timer : TimerBase
    {
        #region Private Members

        /// <summary>
        /// Configuration data for this timer.
        /// </summary>
        private readonly TimerOptions options;

        /// <summary>
        /// The <see cref="TimerStart"/> used to start this timer, or <c>null</c> if the <see cref="TimerBase.State"/>
        /// is <see cref="TimerState.Stopped"/>.
        /// </summary>
        private TimerStart timerStart;

        /// <summary>
        /// The percentage of time left until the timer expires.
        /// </summary>
        /// <remarks>
        /// This field is <c>null</c> if <see cref="SupportsProgress"/> is <c>false</c>.
        /// </remarks>
        private double? timeLeftAsPercentage;

        /// <summary>
        /// The percentage of time elapsed since the timer was started.
        /// </summary>
        /// <remarks>
        /// This field is <c>null</c> if <see cref="SupportsProgress"/> or <see cref="SupportsTimeElapsed"/> is
        /// <c>false</c>.
        /// </remarks>
        private double? timeElapsedAsPercentage;

        /// <summary>
        /// The string representation of the time left until the timer expires.
        /// </summary>
        private string timeLeftAsString;

        /// <summary>
        /// The string representation of the time elapsed since the timer was started.
        /// </summary>
        /// <remarks>
        /// This field is <c>null</c> if <see cref="SupportsTimeElapsed"/> is <c>false</c>.
        /// </remarks>
        private string timeElapsedAsString;

        /// <summary>
        /// The string representation of the time since the timer expired.
        /// </summary>
        private string timeExpiredAsString;

        /// <summary>
        /// The string representation of the time left until the timer expires.
        /// </summary>
        private string timeLeftAsWeekString;

        /// <summary>
        /// The string representation of the time left until the timer expires.
        /// </summary>
        private string timeLeftAsDayString;

        /// <summary>
        /// The string representation of the time left until the timer expires.
        /// </summary>
        private string timeLeftAsHourString;

        /// <summary>
        /// The string representation of the time left until the timer expires.
        /// </summary>
        private string timeLeftAsMinuteString;

        /// <summary>
        /// The string representation of the time left until the timer expires.
        /// </summary>
        private string timeLeftAsSecondString;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer()
            : this(new TimerOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="options">Configuration data for this timer.</param>
        public Timer(TimerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.timerStart = null;
            this.options = TimerOptions.FromTimerOptions(options);

            this.UpdateHourglassTimer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="timerInfo">A <see cref="TimerInfo"/> representing the state of the <see
        /// cref="Timer"/>.</param>
        public Timer(TimerInfo timerInfo)
            : base(timerInfo)
        {
            this.timerStart = TimerStart.FromTimerStartInfo(timerInfo.TimerStart);
            this.options = TimerOptions.FromTimerOptionsInfo(timerInfo.Options) ?? new TimerOptions();

            this.UpdateHourglassTimer();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration data for this timer.
        /// </summary>
        public TimerOptions Options
        {
            get { return this.options; }
        }

        /// <summary>
        /// Gets the <see cref="TimerStart"/> used to start this timer, or <c>null</c> if the <see
        /// cref="TimerBase.State"/> is <see cref="TimerState.Stopped"/>.
        /// </summary>
        public TimerStart TimerStart
        {
            get { return this.timerStart; }
        }

        /// <summary>
        /// Gets the percentage of time left until the timer expires.
        /// </summary>
        /// <remarks>
        /// This property is <c>null</c> if <see cref="SupportsProgress"/> is <c>false</c>.
        /// </remarks>
        public double? TimeLeftAsPercentage
        {
            get { return this.timeLeftAsPercentage; }
        }

        /// <summary>
        /// Gets the percentage of time elapsed since the timer was started.
        /// </summary>
        /// <remarks>
        /// This property is <c>null</c> if <see cref="SupportsProgress"/> or <see cref="SupportsTimeElapsed"/> is
        /// <c>false</c>.
        /// </remarks>
        public double? TimeElapsedAsPercentage
        {
            get { return this.timeElapsedAsPercentage; }
        }

        /// <summary>
        /// Gets the string representation of the time left until the timer expires.
        /// </summary>
        public string TimeLeftAsString
        {
            get { return this.timeLeftAsString; }
        }

        /// <summary>
        /// Gets the string representation of the time elapsed since the timer was started.
        /// </summary>
        /// <remarks>
        /// This property is <c>null</c> if <see cref="SupportsTimeElapsed"/> is <c>false</c>.
        /// </remarks>
        public string TimeElapsedAsString
        {
            get { return this.timeElapsedAsString; }
        }

        /// <summary>
        /// Gets the string representation of the time since the timer expired.
        /// </summary>
        public string TimeExpiredAsString
        {
            get { return this.timeExpiredAsString; }
        }

        /// <summary>
        /// Gets a value indicating whether the timer supports pause.
        /// </summary>
        public bool SupportsPause
        {
            get { return this.TimerStart == null || this.TimerStart.Type == TimerStartType.TimeSpan; }
        }

        /// <summary>
        /// Gets a value indicating whether the timer supports looping.
        /// </summary>
        public bool SupportsLooping
        {
            get { return this.TimerStart == null || this.TimerStart.Type == TimerStartType.TimeSpan; }
        }

        /// <summary>
        /// Gets a value indicating whether the timer supports displaying a progress value.
        /// </summary>
        public bool SupportsProgress
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the timer supports restarting.
        /// </summary>
        public bool SupportsRestart
        {
            get { return this.TimerStart != null && this.TimerStart.Type == TimerStartType.TimeSpan; }
        }

        /// <summary>
        /// Gets a value indicating whether the timer supports displaying the elapsed time since the timer was started.
        /// </summary>
        public bool SupportsTimeElapsed
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the Time Span representation of the time by weeks unit since the timer expired.
        /// </summary>
        public string TimeLeftAsWeekString
        {
            get { return this.timeLeftAsWeekString; }
        }

        /// <summary>
        /// Gets the Time Span representation of the time by days unit since the timer expired.
        /// </summary>
        public string TimeLeftAsDayString
        {
            get { return this.timeLeftAsDayString; }
        }

        /// <summary>
        /// Gets the Time Span representation of the time by hours unit since the timer expired.
        /// </summary>
        public string TimeLeftAsHourString
        {
            get { return this.timeLeftAsHourString; }
        }

        /// <summary>
        /// Gets the Time Span representation of the time by minutes unit since the timer expired.
        /// </summary>
        public string TimeLeftAsMinuteString
        {
            get { return this.timeLeftAsMinuteString; }
        }

        /// <summary>
        /// Gets the Time Span representation of the time by seconds unit since the timer expired.
        /// </summary>
        public string TimeLeftAsSecondString
        {
            get { return this.timeLeftAsSecondString; }
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Returns a <see cref="Timer"/> for a <see cref="TimerInfo"/>.
        /// </summary>
        /// <param name="timerInfo">A <see cref="TimerInfo"/>.</param>
        /// <returns>The <see cref="Timer"/> for the <see cref="TimerInfo"/>.</returns>
        public static Timer FromTimerInfo(TimerInfo timerInfo)
        {
            if (timerInfo == null)
            {
                return null;
            }

            return new Timer(timerInfo);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="newTimerStart">A <see cref="TimerStart"/>.</param>
        /// <returns>A value indicating whether the timer was started successfully.</returns>
        /// <exception cref="ObjectDisposedException">If the timer has been disposed.</exception>
        public bool Start(TimerStart newTimerStart)
        {
            this.ThrowIfDisposed();

            DateTime start = DateTime.Now;
            DateTime end;
            if (newTimerStart != null && newTimerStart.TryGetEndTime(start, out end))
            {
                this.timerStart = newTimerStart;
                this.OnPropertyChanged("TimerStart");

                this.Start(start, end);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Restarts the timer.
        /// </summary>
        /// <returns>A value indicating whether the timer was restarted successfully.</returns>
        /// <exception cref="ObjectDisposedException">If the timer has been disposed.</exception>
        public bool Restart()
        {
            this.ThrowIfDisposed();

            TimerStart timerStart = this.timerStart;
            if (timerStart != null && timerStart.Type == TimerStartType.TimeSpan)
            {
                this.Stop();
                return this.Start(timerStart);
            }

            return false;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string resourceName = string.Format(
                CultureInfo.InvariantCulture,
                "Timer{0}{1}{2}FormatString",
                this.State,
                string.IsNullOrEmpty(this.Options.Title) ? string.Empty : "WithTitle",
                this.Options.LoopTimer && this.SupportsLooping ? "Looped" : string.Empty);

            return string.Format(
                Resources.ResourceManager.GetEffectiveProvider(),
                Resources.ResourceManager.GetString(resourceName) ?? this.GetType().ToString(),
                this.Options.ShowTimeElapsed ? this.TimeElapsed.ToNaturalString() : this.TimeLeft.RoundUp().ToNaturalString(),
                this.TimerStart,
                this.Options.Title);
        }

        /// <summary>
        /// Returns the representation of the <see cref="TimerInfo"/> used for XML serialization.
        /// </summary>
        /// <returns>The representation of the <see cref="TimerInfo"/> used for XML serialization.</returns>
        public override TimerInfo ToTimerInfo()
        {
            TimerInfo timerInfo = base.ToTimerInfo();
            timerInfo.TimerStart = TimerStartInfo.FromTimerStart(this.TimerStart);
            timerInfo.Options = TimerOptionsInfo.FromTimerOptions(this.Options);
            return timerInfo;
        }

        #endregion

        #region Protected Methods (Events)

        /// <summary>
        /// Invoked before the <see cref="TimerBase.Started"/> event is raised
        /// </summary>
        protected override void OnStarted()
        {
            this.UpdateHourglassTimer();
            base.OnStarted();
        }

        /// <summary>
        /// Invoked before the <see cref="TimerBase.Paused"/> event is raised
        /// </summary>
        protected override void OnPaused()
        {
            this.UpdateHourglassTimer();
            base.OnPaused();
        }

        /// <summary>
        /// Invoked before the <see cref="TimerBase.Resumed"/> event is raised
        /// </summary>
        protected override void OnResumed()
        {
            this.UpdateHourglassTimer();
            base.OnResumed();
        }

        /// <summary>
        /// Invoked before the <see cref="TimerBase.Stopped"/> event is raised
        /// </summary>
        protected override void OnStopped()
        {
            this.UpdateHourglassTimer();
            base.OnStopped();
        }

        /// <summary>
        /// Invoked before the <see cref="TimerBase.Expired"/> event is raised
        /// </summary>
        protected override void OnExpired()
        {
            this.UpdateHourglassTimer();
            base.OnExpired();

            if (this.Options.LoopTimer && this.SupportsLooping && this.State != TimerState.Stopped)
            {
                this.Loop();
            }
        }

        /// <summary>
        /// Invoked before the <see cref="TimerBase.Tick"/> event is raised
        /// </summary>
        protected override void OnTick()
        {
            this.UpdateHourglassTimer();
            base.OnTick();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Restarts the timer with the current <see cref="TimerStart"/>.
        /// </summary>
        private void Loop()
        {
            if (!this.EndTime.HasValue || this.EndTime > DateTime.Now)
            {
                throw new InvalidOperationException();
            }

            DateTime now = DateTime.Now;
            DateTime start = this.EndTime.Value;
            DateTime end;

            // Try to find the current loop iteration
            int iteration = 0;
            while (this.TimerStart.TryGetEndTime(start, out end) && end <= now && end > start && iteration++ < 10000)
            {
                // Keep looping
                start = end;
            }

            // Loop if we found the current loop iteration
            if (end > now)
            {
                this.Start(start, end);
            }
        }

        /// <summary>
        /// Updates the <see cref="Timer"/> state.
        /// </summary>
        private void UpdateHourglassTimer()
        {
            this.timerStart = this.State != TimerState.Stopped ? this.timerStart : null;
            this.timeLeftAsPercentage = this.GetTimeLeftAsPercentage();
            this.timeElapsedAsPercentage = this.GetTimeElapsedAsPercentage();
            this.timeLeftAsString = this.GetTimeLeftAsString();
            this.timeLeftAsWeekString = this.GetTimeLeftAsWeekString();
            this.timeLeftAsDayString = this.GetTimeLeftAsDayString();
            this.timeLeftAsHourString = this.GetTimeLeftAsHourString();
            this.timeLeftAsMinuteString = this.GetTimeLeftAsMinuteString();
            this.timeLeftAsSecondString = this.GetTimeLeftAsSecondString();
            this.timeElapsedAsString = this.GetTimeElapsedAsString();
            this.timeExpiredAsString = this.GetTimeExpiredAsString();

            this.OnPropertyChanged("TimerStart", "TimeLeftAsPercentage", "TimeElapsedAsPercentage", "TimeLeftAsString", "TimeElapsedAsString", "TimeExpiredAsString");
        }

        /// <summary>
        /// Returns the percentage of time left until the timer expires.
        /// </summary>
        /// <returns>The percentage of time left until the timer expires.</returns>
        private double? GetTimeLeftAsPercentage()
        {
            if (!this.SupportsProgress || this.State == TimerState.Stopped || !this.TimeElapsed.HasValue || !this.TotalTime.HasValue)
            {
                return null;
            }

            if (this.State == TimerState.Expired)
            {
                return 100.0;
            }

            long timeElapsed = this.TimeElapsed.Value.Ticks;
            long totalTime = this.TotalTime.Value.Ticks;

            if (totalTime == 0)
            {
                return 0.0;
            }

            return 100.0 * timeElapsed / totalTime;
        }

        /// <summary>
        /// Returns the percentage of time elapsed since the timer was started.
        /// </summary>
        /// <returns>The percentage of time elapsed since the timer was started.</returns>
        private double? GetTimeElapsedAsPercentage()
        {
            if (!this.SupportsProgress || !this.SupportsTimeElapsed || this.State == TimerState.Stopped || !this.TimeLeft.HasValue || !this.TotalTime.HasValue)
            {
                return null;
            }

            if (this.State == TimerState.Expired)
            {
                return 0.0;
            }

            long timeLeft = this.TimeLeft.Value.Ticks;
            long totalTime = this.TotalTime.Value.Ticks;

            if (totalTime == 0)
            {
                return 100.0;
            }
            
            return 100.0 * timeLeft / totalTime;
        }

        /// <summary>
        /// Returns the string representation of the time left until the timer expires.
        /// </summary>
        /// <returns>The string representation of the time left until the timer expires.</returns>
        private string GetTimeLeftAsString()
        {
            if (this.State == TimerState.Stopped)
            {
                return Resources.TimerTimerStopped;
            }

            if (this.State == TimerState.Expired)
            {
                return Resources.TimerTimerExpired;
            }

            var weeks = this.TimeLeft?.Duration().Days > 7 ? string.Format("{0:0} week{1}, ", this.TimeLeft?.Days / 7, this.TimeLeft?.Days / 7 == 1 ? string.Empty : "s") : string.Empty;
            var days = this.TimeLeft?.Duration().Days % 7 > 0 ? string.Format("{0:0} day{1}, ", this.TimeLeft?.Days % 7, this.TimeLeft?.Days % 7 == 1 ? string.Empty : "s") : string.Empty;

            string formatted = string.Format("{0}{1}{2}{3}{4}", weeks, days,
            this.TimeLeft?.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", this.TimeLeft?.Hours, this.TimeLeft?.Hours == 1 ? string.Empty : "s") : string.Empty,
            this.TimeLeft?.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", this.TimeLeft?.Minutes, this.TimeLeft?.Minutes == 1 ? string.Empty : "s") : string.Empty,
            this.TimeLeft?.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", this.TimeLeft?.Seconds, this.TimeLeft?.Seconds == 1 ? string.Empty : "s") : string.Empty);
            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);
            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";
            return formatted;
        }

        /// <summary>
        /// Returns the string representation of the time left with week as unit until the timer expires.
        /// </summary>
        /// <returns>The string representation of the time left with week as unit until the timer expires.</returns>
        private string GetTimeLeftAsWeekString()
        {
            if (this.State == TimerState.Stopped)
            {
                return Resources.TimerTimerStopped;
            }

            if (this.State == TimerState.Expired)
            {
                return Resources.TimerTimerExpired;
            }

            string formatted = this.TimeLeft?.Duration().Days > 7 ? string.Format("{0:0} week{1}, ", this.TimeLeft?.Days / 7, this.TimeLeft?.Days / 7 == 1 ? string.Empty : "s") : string.Empty;
            
            if (string.IsNullOrEmpty(formatted)) formatted = "0 week";
            return formatted;
        }

        /// <summary>
        /// Returns the string representation of the time left with day as unit until the timer expires.
        /// </summary>
        /// <returns>The string representation of the time left with day as unit until the timer expires.</returns>
        private string GetTimeLeftAsDayString()
        {
            if (this.State == TimerState.Stopped)
            {
                return Resources.TimerTimerStopped;
            }

            if (this.State == TimerState.Expired)
            {
                return Resources.TimerTimerExpired;
            }

            string formatted = this.TimeLeft?.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", this.TimeLeft?.Days, this.TimeLeft?.Days == 1 ? string.Empty : "s") : string.Empty;

            if (string.IsNullOrEmpty(formatted)) formatted = "0 day";
            return formatted;
        }

        /// <summary>
        /// Returns the string representation of the time left with hour as unit until the timer expires.
        /// </summary>
        /// <returns>The string representation of the time left with hour as unit until the timer expires.</returns>
        private string GetTimeLeftAsHourString()
        {
            if (this.State == TimerState.Stopped)
            {
                return Resources.TimerTimerStopped;
            }

            if (this.State == TimerState.Expired)
            {
                return Resources.TimerTimerExpired;
            }

            var hours = this.TimeLeft?.TotalHours > 0 ? this.TimeLeft?.Days * 24 + this.TimeLeft?.Hours : 0;
            string formatted = hours > 0 ? string.Format("{0:0} hour{1}, ", hours, hours == 1 ? string.Empty : "s") : string.Empty;

            if (string.IsNullOrEmpty(formatted)) formatted = "0 hour";
            return formatted;
        }

        /// <summary>
        /// Returns the string representation of the time left with minute as unit until the timer expires.
        /// </summary>
        /// <returns>The string representation of the time left with minute as unit until the timer expires.</returns>
        private string GetTimeLeftAsMinuteString()
        {
            if (this.State == TimerState.Stopped)
            {
                return Resources.TimerTimerStopped;
            }

            if (this.State == TimerState.Expired)
            {
                return Resources.TimerTimerExpired;
            }

            var minutes = this.TimeLeft?.TotalMinutes > 0 ? (this.TimeLeft?.Days * 24 + this.TimeLeft?.Hours) * 60 + this.TimeLeft?.Minutes : 0;
            string formatted = minutes > 0 ? string.Format("{0:0} minute{1}, ", minutes, minutes == 1 ? string.Empty : "s") : string.Empty;

            if (string.IsNullOrEmpty(formatted)) formatted = "0 minute";
            return formatted;
        }

        /// <summary>
        /// Returns the string representation of the time left with second as unit until the timer expires.
        /// </summary>
        /// <returns>The string representation of the time left with second as unit until the timer expires.</returns>
        private string GetTimeLeftAsSecondString()
        {
            if (this.State == TimerState.Stopped)
            {
                return Resources.TimerTimerStopped;
            }

            if (this.State == TimerState.Expired)
            {
                return Resources.TimerTimerExpired;
            }

            var seconds = this.TimeLeft?.TotalSeconds > 0 ? ((this.TimeLeft?.Days * 24 + this.TimeLeft?.Hours) * 60 + this.TimeLeft?.Minutes) * 60 + this.TimeLeft?.Seconds : 0;
            string formatted = seconds > 1 ? string.Format("{0:0} second{1}, ", seconds, seconds == 1 ? string.Empty : "s") : string.Empty;

            if (string.IsNullOrEmpty(formatted)) formatted = "0 second";
            return formatted;
        }

        /// <summary>
        /// Returns the string representation of the time elapsed since the timer was started.
        /// </summary>
        /// <returns>The string representation of the time elapsed since the timer was started.</returns>
        private string GetTimeElapsedAsString()
        {
            if (!this.SupportsTimeElapsed)
            {
                return null;
            }

            if (this.State == TimerState.Stopped)
            {
                return Resources.TimerTimerStopped;
            }

            if (this.State == TimerState.Expired)
            {
                return Resources.TimerTimerExpired;
            }

            return this.TimeElapsed.ToNaturalString();
        }

        /// <summary>
        /// Returns the string representation of the time since the timer expired.
        /// </summary>
        /// <returns>The string representation of the time since the timer expired.</returns>
        private string GetTimeExpiredAsString()
        {
            if (this.State != TimerState.Expired)
            {
                return Resources.TimerTimerNotExpired;
            }

            return string.Format(
                Resources.ResourceManager.GetEffectiveProvider(),
                Resources.TimerTimeExpiredFormatString,
                this.TimeExpired.ToNaturalString());
        }

        #endregion
    }
}
