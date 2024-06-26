﻿using System;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Xemo.Grip;

namespace Xemo.Azure.Cocoon
{
    public sealed class AzureCocoon<TSchema> : ICocoon
    {
        private readonly TableEntity azureEntity;
        private readonly TSchema schema;

        public AzureCocoon(TableEntity azureEntity, TSchema schema)
        {
            this.azureEntity = azureEntity;
            this.schema = schema;
        }

        public IGrip Grip() =>
            new AsGrip(
                this.azureEntity.PartitionKey,
                this.azureEntity.RowKey
            );

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            throw new NotImplementedException();
        }

        public ICocoon Mutate<TPatch>(TPatch mutation)
        {
            throw new NotImplementedException();
        }

        public ICocoon Schema<TSchema>(TSchema schema)
        {
            throw new NotImplementedException();
        }
    }
}

