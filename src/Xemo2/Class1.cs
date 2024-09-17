﻿namespace Xemo2;

public interface IPart<TContent>
{
    IPart<TContent> Refine(Func<TContent, TContent> content);
    TContent Pick();
}