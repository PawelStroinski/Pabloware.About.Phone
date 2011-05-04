using System.IO;
using System;
using System.Net;
using System.Windows;

namespace Dietphone.Models
{
    public class Category : Entity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
