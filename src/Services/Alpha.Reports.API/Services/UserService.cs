using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Services;

public interface IUserService
{
    UserInfoModel GetUserInfo(int custId, byte settingStatus);
}

public class UserService : IUserService
{
    private readonly IDynamicRepository dynamicRepository;

    public UserService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public UserInfoModel GetUserInfo(int custId, byte settingStatus)
    {
        var criteria = new GetUserInfoCriteria(custId, settingStatus);

        return dynamicRepository.Fetch<UserInfoModel>(criteria).FirstOrDefault();
    }
}