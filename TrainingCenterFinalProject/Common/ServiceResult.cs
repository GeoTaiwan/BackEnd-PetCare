namespace TrainingCenterFinalProject.Common;

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    // 專門為了第三方登入提示所設計的特殊欄位
    public string AlertMessage { get; set; }
    public T Data { get; set; }
}