using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

class Automata {
	static List<int> LETRA = new List<int> {65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,95};
	static List<int> DIGITO = new List<int> {48,49,50,51,52,53,54,55,56,57};
static void Main(string[] args) {
	int estado = 1;
	string auxiliar = "";
	string path = @"";
	Console.WriteLine("Ingrese la direccion del archivo de entrada: ");
	path += Console.ReadLine();
	string cadena = System.IO.File.ReadAllText(path);

	for(int i = 0; i < cadena.Length; i++) {
		switch(estado){
			case 1:
				if(DIGITO.Contains(cadena[i])){
					estado = 2;
					auxiliar += cadena[i];
				}
				else if(cadena[i] == 61){
					estado = 3;
					auxiliar += cadena[i];
				}
				else if(cadena[i] == 58){
					estado = 4;
					auxiliar += cadena[i];
				}
				else if(LETRA.Contains(cadena[i])){
					estado = 5;
					auxiliar += cadena[i];
				}
				else if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){
					//obviar los espacios
				}
				else{
					Console.WriteLine(cadena[i] + ", no. de token: 54");
					estado = 1;
				}
			break;
			case 2:
				if(DIGITO.Contains(cadena[i])){
					estado = 2;
					auxiliar += cadena[i];
				}
				else if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){
					Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
					auxiliar = "";
					estado = 1;
				}
				else{
					if(validacion(cadena[i]) == false){
						auxiliar += cadena[i];
						Console.WriteLine(auxiliar + ", no. de token: 54");
						auxiliar = "";
						estado = 1;
					}
					else{
						//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado
						Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
						estado = 1;
						i--;
						auxiliar = "";
					}
				}
			break;
			case 3:
					if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){
						Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
						auxiliar = "";
						estado = 1;
					}
					//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado
					else{
						Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
						estado = 1;
						i--;
						auxiliar = "";
					}
			break;
			case 4:
				if(cadena[i] == 61){
					estado = 3;
					auxiliar += cadena[i];
				}
				else if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){
					Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
					auxiliar = "";
					estado = 1;
				}
				else{
					if(validacion(cadena[i]) == false){
						auxiliar += cadena[i];
						Console.WriteLine(auxiliar + ", no. de token: 54");
						auxiliar = "";
						estado = 1;
					}
					else{
						//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado
						Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
						estado = 1;
						i--;
						auxiliar = "";
					}
				}
			break;
			case 5:
				if(DIGITO.Contains(cadena[i])){
					estado = 5;
					auxiliar += cadena[i];
				}
				else if(LETRA.Contains(cadena[i])){
					estado = 5;
					auxiliar += cadena[i];
				}
				else if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){
					Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
					auxiliar = "";
					estado = 1;
				}
				else{
					if(validacion(cadena[i]) == false){
						auxiliar += cadena[i];
						Console.WriteLine(auxiliar + ", no. de token: 54");
						auxiliar = "";
						estado = 1;
					}
					else{
						//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado
						Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
						estado = 1;
						i--;
						auxiliar = "";
					}
				}
			break;
		}
	}

	Console.WriteLine(auxiliar + ", TOKEN: " + obtenerToken(estado, auxiliar));
	Console.WriteLine("\nTERMINADO");
	Console.ReadLine();

}


	static int obtenerToken(int estado, string auxiliar){
	Dictionary<int, string> actions = new Dictionary<int, string>()
	{
		{18, "PROGRAM"},
		{19, "INCLUDE"},
		{20, "CONST"},
		{21, "TYPE"},
		{22, "VAR"}
	};

		int response = 54;
		if(actions.Values.Contains(auxiliar.ToUpper())){
			for(int i = 0; i < actions.Count; i++){
				if(actions.ElementAt(i).Value.Equals(auxiliar.ToUpper())){
					response = actions.ElementAt(i).Key;
				}
			}
		} else {
			switch(estado){
				case 2:
					response = 1; break;
				case 3:
					if(auxiliar == "="){
						response = 2;
					}
					if(auxiliar == ":="){
						response = 3;
					}
					break;
				case 5:
					response = 4; break;
			}
		}
		return response; 
	}


	static bool validacion(char cadena){
		bool response = false;
		if(LETRA.Contains(cadena) || DIGITO.Contains(cadena)){
			response = true;
		}
		return response;
	}

}
