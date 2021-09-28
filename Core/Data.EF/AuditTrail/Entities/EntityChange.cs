﻿using CoreApp.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreApp.Data.AuditTrail
{
    [Table("EntityChange", Schema = "audittrail")]
    public class EntityChange : IIdEntity
    {
        public Guid Id { get; set; }
        [StringLength(20)]
        public string ChangeType { get; set; }
        [StringLength(255)]
        public string ChangeReason { get; set; }
        public Guid ChangeToken { get; set; }
        [StringLength(50)]
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public string Changes { get; set; }

        [StringLength(255)]
        public string EntityType { get; set; }
        public string EntityId { get; set; }
    }
}
