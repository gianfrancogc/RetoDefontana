
USE Prueba
GO

 --•	El total de ventas de los últimos 30 días (monto total y cantidad total de ventas).
SELECT COUNT(*) AS TotalVentas, SUM(Total) AS MontoTotal
FROM venta
WHERE fecha >= DATEADD(day, -30, GETDATE());

--•	El día y hora en que se realizó la venta con el monto más alto (y cuál es aquel monto).
SELECT TOP(1) fecha, Total AS MontoVenta
FROM venta
ORDER BY Total DESC;

--•	Indicar cuál es el producto con mayor monto total de ventas. 

 SELECT TOP(1) p.ID_Producto AS Producto, p.Nombre, SUM(vd.TotalLinea) AS MontoTotalVentas
FROM producto p
JOIN VentaDetalle vd ON p.ID_Producto = vd.ID_Producto
GROUP BY p.ID_Producto , p.Nombre
ORDER BY MontoTotalVentas DESC

--•	Indicar el local con mayor monto de ventas.
SELECT  l.nombre AS [Local], SUM(v.Total) AS MontoTotalVentas
FROM [Local] l
JOIN venta v ON l.id_local = v.id_local
GROUP BY l.nombre
ORDER BY MontoTotalVentas DESC;

--•	¿Cuál es la marca con mayor margen de ganancias?

SELECT TOP(1) m.nombre AS Marca, (SUM(vd.Precio_Unitario) - SUM(p.Costo_Unitario)) AS MargenGanancias
FROM marca m
JOIN producto p ON m.id_marca = p.id_marca
JOIN VentaDetalle vd ON p.id_producto = vd.id_producto
GROUP BY m.nombre
ORDER BY MargenGanancias DESC;

--•	¿Cómo obtendrías cuál es el producto que más se vende en cada local?


WITH CTE AS (
    SELECT
	    L.ID_Local AS idLocal,
        L.Nombre AS Local,
        P.Nombre AS Producto,
        SUM(VD.Cantidad) AS CantidadTotal,
        ROW_NUMBER() OVER (PARTITION BY L.ID_Local ORDER BY SUM(VD.Cantidad) DESC) AS RowNum
    FROM
        VentaDetalle VD
        INNER JOIN Venta V ON VD.ID_Venta = V.ID_Venta
        INNER JOIN Local L ON V.ID_Local = L.ID_Local
        INNER JOIN Producto P ON VD.ID_Producto = P.ID_Producto
    WHERE
        V.Fecha >= DATEADD(DAY, -30, GETDATE()) 
    GROUP BY
        L.ID_Local, L.Nombre, P.Nombre
)
SELECT
idLocal,
    Local,
    Producto
	FROM
    CTE
WHERE
    RowNum = 1;





