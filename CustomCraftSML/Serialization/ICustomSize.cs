﻿namespace CustomCraft2SML.Serialization
{
    public interface ICustomSize
    {
        short Height { get; }
        TechType ItemID { get; }
        short Width { get; }
    }
}