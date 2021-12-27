﻿namespace OkayegTeaTime.Exceptions;

public class TooManyReminderException : Exception
{
    public override string Message { get; } = "too many reminders set for that person";

    public TooManyReminderException() : base()
    {
    }

    public TooManyReminderException(string message) : base(message)
    {
    }
}
