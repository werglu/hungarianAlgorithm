# Hungarian Algorithm

Zaprojektować i zaimplementować algorytm znajdujący
najtańsze (sumaryczne) doprowadzenie wody do domów. Dane jest n studni i kn
domów (punkty na płaszczyźnie), każda studnia zapewnia wodę k domom,
koszt - odległość euklidesowa. 
Na wejściu dostajemy graf dwudzielny G(U,V,E), |V|=n, |U|=kn
Wierzchołki należące do V reprezentują studnie, a do U domy. Istnieją krawędzie z każdego domu do każdej studni.
Chcemy zminimalizować sumę wag krawędzi, tak aby każdy dom był połączony z dokładnie jedną studnią.
