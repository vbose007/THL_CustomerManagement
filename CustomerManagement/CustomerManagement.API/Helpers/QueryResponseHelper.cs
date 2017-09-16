using System;
using System.Linq.Expressions;
using System.Text;
using CustomerManagement.API.Constants;
using CustomerManagement.API.Models.RequestResponseModels;
using CustomerManagement.Data;

namespace CustomerManagement.API.Helpers
{
    public class QueryResponseHelper
    {
        private IHttpContextHelper _httpHelper;

        public QueryResponseHelper(IHttpContextHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }


        public string ComposeGetUrl(GetCustomersRequestModel query, int pageNumber)
        {
            string scheme = _httpHelper.Scheme;
            string schemeDelimiter = _httpHelper.SchemeDelimiter;
            string host = _httpHelper.Host;
            string portNumber = _httpHelper.Port != 80 ? $":{_httpHelper.Port}" : string.Empty;
            string path = _httpHelper.Path;

            StringBuilder pageUrl = new StringBuilder($"{scheme}{schemeDelimiter}{host}{portNumber}{path}");

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                pageUrl.Append($"name={query.Name}");
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                pageUrl.Append($"&email={query.Email}");
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                pageUrl.Append($"&sortBy={query.SortBy}");
            }

            pageUrl.Append($"&sortAsc={query.SortAsc}");

            var pageSize = query.PageSize < 1 ? ApplicationConstants.DefaultPageSize : query.PageSize;

            if (pageNumber < 1) pageNumber = ApplicationConstants.DefaultPageNumber;

            pageUrl.Append($"&pageSize={pageSize}");
            pageUrl.Append($"&pageNumber={pageNumber}");


            return pageUrl.ToString();
        }

        public Expression<Func<Customer, object>> ComposeSortByClause(GetCustomersRequestModel query)
        {
            Expression<Func<Customer, object>> sortBy = null;
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                var sortByField = query.SortBy.ToLower();

                switch (sortByField)
                {
                    case "id":
                        sortBy = c => c.Id;
                        break;
                    case "name":
                        sortBy = c => c.Name;
                        break;
                    case "email":
                        sortBy = c => c.EmailAddress;
                        break;

                    default:
                        sortBy = c => c.Id;
                        break;
                }
            }

            return sortBy;
        }

        public Expression<Func<Customer, bool>> ComposeWhereCondition(GetCustomersRequestModel query)
        {
            Expression<Func<Customer, bool>> @where = null;
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var name = query.Name.ToLower();
                if (!string.IsNullOrWhiteSpace(query.Email))
                {
                    var email = query.Email.ToLower();

                    @where = c => c.Name.ToLower().Contains(name)
                                  && c.EmailAddress.ToLower().Contains(email);
                }
                else
                {
                    @where = c => c.Name.ToLower().Contains(name);
                }
            }
            else if (!string.IsNullOrWhiteSpace(query.Email))
            {
                var email = query.Email.ToLower();

                @where = c => c.EmailAddress.ToLower().Contains(email);
            }

            return @where;
        }
    }
}