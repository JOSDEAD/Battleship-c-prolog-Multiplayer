
hayBarco(0,[L|_]):- L == 1.
hayBarco(P,[_|C]):- P1 is P - 1, hayBarco(P1,C).

comprobar(I,J,F):- consult(F), fila(I,L), hayBarco(J,L).

