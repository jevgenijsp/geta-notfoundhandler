﻿// Copyright (c) Geta Digital. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Geta.NotFoundHandler.Data;

namespace Geta.NotFoundHandler.Core.Providers.RegexRedirects;

public class DefaultRegexRedirectsService : IRegexRedirectsService
{
    private readonly RegexRedirectFactory _regexRedirectFactory;
    private readonly IRepository<RegexRedirect> _repository;
    private readonly IRegexRedirectLoader _redirectLoader;
    private readonly IRegexRedirectOrderUpdater _orderUpdater;

    public DefaultRegexRedirectsService(
        RegexRedirectFactory regexRedirectFactory,
        IRepository<RegexRedirect> repository,
        IRegexRedirectLoader redirectLoader,
        IRegexRedirectOrderUpdater orderUpdater)
    {
        _regexRedirectFactory = regexRedirectFactory;
        _repository = repository;
        _redirectLoader = redirectLoader;
        _orderUpdater = orderUpdater;
    }

    public void Create(string oldUrlRegex, string newUrlFormat, int orderNumber)
    {
        var regexRedirect = _regexRedirectFactory.CreateNew(oldUrlRegex, newUrlFormat, orderNumber);
        _repository.Save(regexRedirect);
        _orderUpdater.UpdateOrder();
    }

    public void Update(Guid id, string oldUrlRegex, string newUrlFormat, int orderNumber)
    {
        var original = _redirectLoader.Get(id);
        var isIncrease = original.OrderNumber < orderNumber;
        var regexRedirect = _regexRedirectFactory.Create(id, oldUrlRegex, newUrlFormat, orderNumber);
        _repository.Save(regexRedirect);
        _orderUpdater.UpdateOrder(isIncrease);
    }

    public void Delete(Guid id)
    {
        var regexRedirect = _regexRedirectFactory.CreateForDeletion(id);
        _repository.Delete(regexRedirect);
        _orderUpdater.UpdateOrder();
    }
}
