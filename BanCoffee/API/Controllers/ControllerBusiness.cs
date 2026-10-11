using Microsoft.AspNetCore.Mvc;
using System;

namespace API
{
    public abstract class ControllerBusiness<Business>(Business business) : ControllerBase
    {
        protected readonly Business _Business = business;
        protected IActionResult Execute(Func<IActionResult> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        protected T Execute<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
