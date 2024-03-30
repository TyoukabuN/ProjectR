//-----------------------------------------------------------------------
// <copyright file="IAskIfCanFormatTypes.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
#pragma warning disable

    using System;

    public interface IAskIfCanFormatTypes
    {
        bool CanFormatType(Type type);
    }
}