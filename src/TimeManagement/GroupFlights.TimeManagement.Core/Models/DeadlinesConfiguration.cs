﻿namespace GroupFlights.TimeManagement.Core.Models;

public record DeadlinesConfiguration(IReadOnlyCollection<NotificationOffset> NotificationOffsets);