using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epica.Web.Operacion.Models.Common;

/// <summary>
/// ViewModel de regresar una respuesta al frontend
/// </summary>
/// 
public class ResponseViewModel
{
    #region Propiedades
    public bool IsSuccessFully { get; set; }
    public string Message { get; set; }
    public object Result { get; set; }
    public string Url { get; set; }
    #endregion

    #region Constructores
    /// <summary>
    /// Constructor vacio.
    /// </summary>
    public ResponseViewModel() { }

    /// <summary>
    /// Constructor solo para enviar una sola respuesta.
    /// </summary>
    /// <param name="isSucces">True || False</param>
    public ResponseViewModel(bool isSucces)
    {
        IsSuccessFully = isSucces;
    }

    /// <summary>
    /// Constructor solo para enviar una sola respuesta.
    /// </summary>
    /// <param name="isSucces">True || False</param>
    /// <param name="result">Objecto de tipo Json</param>
    public ResponseViewModel(bool isSucces, object result)
    {
        IsSuccessFully = isSucces;
        Result = result;
    }

    /// <summary>
    /// Constructor solo para enviar una sola respuesta con su posible error.
    /// </summary>
    /// <param name="isSucces">True || False</param>
    public ResponseViewModel(bool isSucces, string msj)
    {
        IsSuccessFully = isSucces;
        Message = msj;
    }

    /// <summary>
    /// Constructor solo para enviar una sola respuesta con su posible error.
    /// </summary>
    /// <param name="isSucces">True || False</param>
    /// <param name="result">Objecto tipo Json</param>
    /// <param name="msj">Msj de response</param>
    public ResponseViewModel(bool isSucces, object result, string msj)
    {
        IsSuccessFully = isSucces;
        Result = result;
        Message = msj;
    }

    /// <summary>
    /// Constructor que recibe una exepcion 
    /// </summary>
    /// <param name="exception">Execption de un error.</param>
    public ResponseViewModel(Exception exception)
    {
        IsSuccessFully = false;
        Message = $"Ocurrio un error en :  {exception.Message}";
    }
    #endregion
}
