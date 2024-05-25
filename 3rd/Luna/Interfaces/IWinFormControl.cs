namespace Luna.Interfaces
{
    public interface IWinFormControl<TResult>
    {
        /// <summary>
        /// 用于输入、输出控件
        /// </summary>
        /// <returns>获取控件选中或输入的内容</returns>
        TResult GetResult();
    }
}
