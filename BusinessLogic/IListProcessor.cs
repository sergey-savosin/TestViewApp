namespace TestViewApp.BusinessLogic
{
    public interface IListProcessor<TResult>
    {
        public Task<TResult[]> GetListAsync(string path);
    }
}
