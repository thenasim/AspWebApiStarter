using System;
using Application.Interfaces;

namespace Application.Service
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}