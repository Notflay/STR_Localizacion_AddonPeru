CREATE PROCEDURE STR_SP_Valida_AppStrat
(
	IN object_type  NVARCHAR(20),
	IN transaction_type  NVARCHAR(1),
	IN list_of_cols_val_tab_del nvarchar(255),
	OUT error INTEGER,
 	OUT error_message NVARCHAR(200)
)
--RETURNS VARCHAR(200)
AS

--r VARCHAR(200);
user VARCHAR(20);
R1 VARCHAR(30);
R2 VARCHAR(30);
R3 VARCHAR(30);
R4 VARCHAR(30);
R5 VARCHAR(30);
DOCTYPE NCHAR(1);

DATA INTEGER; 
R6 INTEGER; 
R7 INTEGER; 
R8 INTEGER; 
R9 INTEGER;
R10 INTEGER;
Campovalida INTEGER;
TpTr int;
DcOP int;

BEGIN
error :=0;
error_message := N'Ok';

	--IF :TIPO = 'VENTAS'
	--THEN
		--FACTURA / NOTA DEBITO PROVEEDORES (OINV)
		IF :object_type = '13' AND (:transaction_type = 'A' or :transaction_type = 'U')
		THEN
			-- Indicador de Factura
			SELECT(SELECT count(*) FROM OINV T0 
			WHERE IFNULL(T0."U_BPP_MDTD",'')='' and T0."DocSubType" ='--'
			 AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R1 FROM DUMMY;
			
			-- Indicador de Nota de Debito
			SELECT(SELECT count(*) FROM OINV T0 
			WHERE IFNULL(T0."U_BPP_MDTD",'') ='' and  T0."DocSubType" ='DN'
			AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R2 FROM DUMMY;
			
			-- Indicador de Boleta
			SELECT(SELECT count(*) FROM OINV T0 
			WHERE IFNULL(T0."U_BPP_MDTD",'')='' and T0."DocSubType" = 'IB' 
			AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R3 FROM DUMMY;
			
			--Tipo de Operacion	
			SELECT(select A."DocType" from oinv A where A."DocEntry" = :list_of_cols_val_tab_del )INTO DOCTYPE FROM DUMMY;
			
			IF :DOCTYPE='I'	  
			THEN
				SELECT(SELECT count(*) FROM INV1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
				WHERE (IFNULL(T0."U_tipoOpT12",'') ='' and T1."InvntItem"='Y' ) 
				AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R4 FROM DUMMY;
	
			  	IF :R1 > 0 THEN 
			  		error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT'; 
			    END IF;
			  	IF :R2 > 0 THEN 
			  		error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT'; 
			  	END IF;
			  	IF :R3 > 0 THEN 
			  		error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT'; 
			  	END IF;
			  	IF :R4 > 0 THEN 
			  		error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento'; 
			  	END IF;
			END IF;
	
		--OBJETO ENTREGA (ODLN)                   
			IF :object_type = '15' AND (:transaction_type = 'A' or :transaction_type = 'U')
			THEN
				-- Indicador de la Entrega
				SELECT(SELECT count(*) FROM ODLN T0 
				WHERE IFNULL(T0."U_BPP_MDTD",'') ='' /*OR T0.Indicator <>'09'*/
				AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R1 FROM DUMMY;
		
			--Valida el tipo de salida de documento
			--DECLARE @DATA INT, @R6 INT, @R7 INT, @R8 INT, @R9 INT, @R10 INT
			
				SELECT(SELECT count(*) FROM ODLN T0 WHERE  T0."U_BPP_MDTS"='TSE' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO DATA FROM DUMMY;
			
				IF :DATA >0
				THEN
					--Valida el Nombre de Transportista
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDNT",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R2 FROM DUMMY;
					--Valida el Direccion del Transportista
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDDT",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R3 FROM DUMMY;
					--Valida el RUC del transportista
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDRT",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R4 FROM DUMMY;		
					--Valida el Nombre del conductor
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDFN",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R5 FROM DUMMY;		
					--Valida la Marca del vehiculo
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDVN",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R6 FROM DUMMY;		
					--Valida la Licencia del conductor
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDFC",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R7 FROM DUMMY;		
					--Valida la placa del Vehiculo
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDVC",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R8 FROM DUMMY;		
					--Valida la Placa de la Tolva
					SELECT(SELECT count(*) FROM ODLN T0 
								WHERE IFNULL(T0."U_BPP_MDVT",'') ='' AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R9 FROM DUMMY;		
				END IF;
			
				--Tipo de Operacion	
				SELECT(select A."DocType" from ODLN A where A."DocEntry" =:list_of_cols_val_tab_del )INTO DOCTYPE FROM DUMMY;
				
				IF :DOCTYPE='I'
				THEN	  
					SELECT(SELECT count(*) FROM DLN1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
					WHERE (IFNULL(T0."U_tipoOpT12",'') ='' and T1."InvntItem"='Y' ) AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R10 FROM DUMMY;
				
				    IF :R1 > 0 THEN 
				    	error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT'; 
				    END IF;
				   	IF :R2 > 0 THEN 
				   		error_message := 'STR_A: Ingrese el Nombre del Transportista'; 
				   	END IF;
				  	IF :R3 > 0 THEN 
				  		error_message := 'STR_A: Ingrese la Dirección del Transportista'; 
				  	END IF;
				    IF :R4 > 0 THEN 
				    	error_message := 'STR_A: Ingrese la RUC del Transportista'; 
				    END IF;
				    IF :R5 > 0 THEN 
				    	error_message := 'STR_A: Ingrese la Nombre del transportista'; 
				    END IF;
				    IF :R6 > 0 THEN 
				    	error_message := 'STR_A: Ingrese la Marca del vehiculo'; 
				    END IF;
				    IF :R7 > 0 THEN 
				    	error_message := 'STR_A: Ingrese la Licencia del conductor'; 
				    END IF;
				  	IF :R8 > 0 THEN 
				  		error_message := 'STR_A: Ingrese la Placa del vehiculo'; 
				  	END IF;
				  	IF :R9 > 0 THEN 
				  		error_message := 'STR_A: Ingrese Placa de la tolva'; 
				  	END IF;
				  	IF :R10 > 0 THEN 
				  		error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento'; 
				  	END IF;
				END IF;                     
	
			--OBJETO NOTA DE CREDITO (ORIN) 
				IF :object_type = '14' AND (:transaction_type = 'A' or :transaction_type = 'U')
				THEN
				 -- Indicador de la Nota de Credito
					SELECT(SELECT count(*) FROM ORIN T1 WHERE IFNULL(T1."U_BPP_MDTD",'')=''
					and T1."DocEntry" = :list_of_cols_val_tab_del)INTO R1 FROM DUMMY;
				
				--Tipo de Operacion	
					SELECT(select A."DocType" from ORIN A where A."DocEntry" =:list_of_cols_val_tab_del )INTO DOCTYPE FROM DUMMY;
					
					IF :DOCTYPE='I'	  
					THEN
						SELECT(SELECT count(*) FROM RIN1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
						WHERE (IFNULL(T0."U_tipoOpT12",'')='' and T1."InvntItem"='Y' ) 
						AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R2 FROM DUMMY;
												
						IF :R1 > 0 THEN 
							error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT';						
					    END IF;
						IF :R2 > 0 THEN 
							error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento';
						END IF;
					END IF;
				END IF;
		
				--OBJETO DEVOLUCION (ORDN) 
				IF :object_type = '16' AND (:transaction_type = 'A' or :transaction_type = 'U')
				THEN
					--Tipo de Operacion	
					SELECT (select A."DocType" from ORDN A where A."DocEntry" =:list_of_cols_val_tab_del )INTO DOCTYPE FROM DUMMY;
					IF :DOCTYPE='I'	  
					THEN
						SELECT(SELECT count(*) FROM RDN1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
						WHERE (IFNULL(T0."U_tipoOpT12",'') ='' and T1."InvntItem"='Y' ) 
						AND T0."DocEntry" = :list_of_cols_val_tab_del) INTO R2 FROM DUMMY;
					
						 IF :R2 > 0 THEN 
						 	error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento'; 
						 END IF;
					END IF;
				END IF;
			END IF;
		END IF;
	--END IF; -- FIN VENTAS

      --FACTURA DE PROVEEDORES (OPCH)
      
      IF :object_type = '18' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            --Indicador de la Factura de Proveedor
            select (SELECT count(*) FROM OPCH T0 
            WHERE ifnull(T0."U_BPP_MDTD",'') ='' 
            AND T0."DocEntry" = :list_of_cols_val_tab_del) into R1 from dummy;
            --Tipo de Operacion     
            select  (select A."DocType"  from OPCH A where A."DocEntry" = :list_of_cols_val_tab_del ) into DOCTYPE FROM DUMMY;
            IF :DOCTYPE='I'
            THEN     
                  
                   select (SELECT count(*) FROM PCH1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
                   WHERE ifnull(T0."U_tipoOpT12",'') ='' and t1."InvntItem"='Y' 
                    AND T0."DocEntry" = :list_of_cols_val_tab_del) into R4 from dummy;
                   
                   IF :R1 > 0 THEN 
                   error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT'; 
                   END if;
                   IF :R4 > 0 then
                   error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento';
                    END if;
               END IF;
      END if;

      --NOTA DE CREDITO (ORPC)
      IF :object_type = '19' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            --Indicador de la Nota de Credito
            select  (SELECT count(*) FROM ORPC T0 
            WHERE ifnull(T0."U_BPP_MDTD",'') ='' 
            AND T0."DocEntry" = :list_of_cols_val_tab_del) INTO R1 from dummy;
            --Tipo de Operacion     
            select  (select A."DocType"  from ORPC A where A."DocEntry" = :list_of_cols_val_tab_del ) into DOCTYPE from dummy;
            IF :DOCTYPE='I'
            then     
                 
            select (SELECT count(*) FROM RPC1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
            WHERE ifnull(T0."U_tipoOpT12",'') ='' and t1."InvntItem"='Y'  
            AND T0."DocEntry" = :list_of_cols_val_tab_del) into R4 from dummy;
            
              IF :R1 > 0
               then  error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT';
                END if;
              IF :R4 > 0 
              then error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento';
               END if;
           end if;
      END if;
-----------------------------------------++++++++++++++++++++++++++++++++++++++++
      --PAGO EFECTUADO
      --Valida medio de Pago
      IF :object_type = '46' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            select (SELECT count(*) FROM OVPM T0 
            WHERE T0."U_BPP_MPPG" ='000'
           and t0."DataSource" <> 'O' AND T0."DocEntry" = :list_of_cols_val_tab_del) into R1 from dummy;
            
              IF :R1 > 0 then  
              error_message := 'STR_A: Ingrese el Medio de Pago SUNAT';
               END if;
      END if;
      
      


      --PAGO RECIBIDO
      IF :object_type = '24' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            select (SELECT count(*) FROM ORCT T0 
            WHERE T0."U_BPP_MPPG" ='000'  and t0."DataSource"<> 'O'
             AND T0."DocEntry" = :list_of_cols_val_tab_del) into R1 from dummy;
            IF :R1 > 0
             then 
             error_message := 'STR_A: Ingrese el Medio de Pago SUNAT'; 
             END if;
      END if;
      
      
      --Valida que se ingrese el indicador en Facturas de Anticipo Ventas
      IF :object_type = '203' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            select (select count(*) from ODPI T0
            WHERE ifnull(T0."U_BPP_MDTD",'') = ''  AND T0."DocEntry" =:list_of_cols_val_tab_del) into R1 from dummy;
            
             IF :R1 > 0 
             then 
             error_message := 'STR_A: Debe seleccionar el tipo de documento SUNAT';
              END if;
      END if;
      
      --Valida que se ingrese la serie y el correlativo de documento si el indicador no empieza con "Z"
      IF :object_type = '18' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            select  (select count(*) from OPCH T0
                  WHERE ifnull(T0."U_BPP_MDSD",'') = '' 
                  OR ifnull(T0."U_BPP_MDCD",'') = '' 
                  AND T0."DocEntry" = :list_of_cols_val_tab_del) into R1 from dummy;
            
             IF :R1 > 0 
             then 
             error_message := 'STR_A: Debe ingresar la serie y el número SUNAT';
             END if;
      END if;
      
      --Valida DUA

      IF :object_type = '18' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            select (select count(*) from OPCH T0
                  WHERE (ifnull(T0."U_BPP_MDND",'') = '' 
                  OR ifnull(T0."U_BPP_MDFD",'') = '') 
                  AND T0."DocEntry" = :list_of_cols_val_tab_del
                  AND T0."U_BPP_MDTD" = '50') into R1 from dummy;
            
              IF :R1 > 0 
              then  
              error_message := 'STR_A: Ingrese los datos de DUA';
               END if;
      END if;
   
    --Valida que se ingrese la serie y el correlativo de documento si el indicador no empieza con "Z"
      IF :object_type = '204' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            select (select count(*) from ODPO T0
                  WHERE ifnull(T0."U_BPP_MDSD",'') = '' 
                  OR ifnull(T0."U_BPP_MDCD",'') = '' 
                  AND T0."DocEntry" =:list_of_cols_val_tab_del) into R1 from dummy;
            
            IF :R1 > 0 
            then
            error_message := 'STR_A: Debe ingresar la serie y el número SUNAT';
             END if;
      END if;
      
      --Valida DUA
      IF :object_type = '204' AND (:transaction_type = 'A' or :transaction_type = 'U')
      then
            select (select count(*) from ODPO T0
                  WHERE (ifnull(T0."U_BPP_MDND",'') = '' 
                  OR ifnull(T0."U_BPP_MDFD",'') = '') 
                  AND T0."DocEntry" = :list_of_cols_val_tab_del
                  AND T0."U_BPP_MDTD" = '50') into R1 from dummy;
            
            IF :R1 > 0
             then 
             error_message := 'STR_A: Ingrese los datos de DUA';
             END if;
      END if;
-----------------------------------------+++++++++++++++++++++++++++++++++++++++++


	--OBJETO ENTRADA (OIGN) 
	IF :object_type = '59' AND (:transaction_type = 'A' or :transaction_type = 'U')
	THEN
		--Tipo de Operacion	
		SELECT(SELECT count(*) FROM IGN1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
		WHERE (IFNULL(T0."U_tipoOpT12",'') ='' and t1."InvntItem"='Y' ) 
		AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R4 FROM DUMMY;
		
		IF :R4 > 0 THEN 
			error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento'; 
		END IF;
	END IF;

--OBJETO SALIDA (OIGE)
	IF :object_type = '60' AND (:transaction_type = 'A' or :transaction_type = 'U')
	THEN
		--Tipo de Operacion	
		SELECT(SELECT count(*) FROM IGE1 T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
		WHERE (IFNULL(T0."U_tipoOpT12",'') ='' and T1."InvntItem"='Y' ) 
		AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R4 FROM DUMMY;
		
		IF :R4 > 0 THEN 
			error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento'; 
		END IF;
	END IF;
	--OBJETO TRANSFERENCIA DE STOCK (OWTR)
	IF :object_type = '67' AND (:transaction_type = 'A' or :transaction_type = 'U')
	THEN
			--Tipo de Operacion	
		SELECT(SELECT count(*) FROM WTR1  T0  INNER JOIN OITM T1 ON T0."ItemCode" = T1."ItemCode"
		WHERE (IFNULL(T0."U_tipoOpT12",'') ='' and t1."InvntItem"='Y' ) 
		AND T0."DocEntry" = :list_of_cols_val_tab_del)INTO R4 FROM DUMMY;
		
		IF :R4 > 0 THEN 
			error_message := 'STR_A: Ingrese el Tipo de Operacion en el detalle del documento'; 
		END IF;
	
	END IF;


	IF :error_message = 'Ok'
	THEN 
	 SELECT :error, :error_message from dummy;
	ELSE 
	 error := 1;
	SELECT :error, :error_message from dummy;
	end if;

END;