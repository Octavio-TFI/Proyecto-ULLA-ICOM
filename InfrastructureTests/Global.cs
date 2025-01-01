﻿global using Domain.Entities;
global using Domain.Events;
global using Domain.Repositories;
global using Domain.ValueObjects;
global using Infrastructure.Database;
global using Infrastructure.Outbox;
global using Infrastructure.Outbox.Abstractions;
global using MediatR;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Mock;
global using Moq;
global using Newtonsoft.Json;
global using NUnit.Framework;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;