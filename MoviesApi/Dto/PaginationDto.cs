namespace MoviesApi.Dto;

public class PaginationDto
{
    private readonly int _amountMaxRegistersByPage = 50;
    private int _amountRegistersByPage = 10;
    public int Page { get; set; } = 1;

    public int AmountRegisterByPage
    {
        get => _amountRegistersByPage;
        set => _amountRegistersByPage = value > _amountMaxRegistersByPage ? _amountMaxRegistersByPage : value;
    }
}