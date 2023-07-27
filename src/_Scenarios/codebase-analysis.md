- debugowanie _Scenarios
   - posiłkować się analizą na miro (na miro "Strategiczne DDD")
   - analizować publikowane events
   - analizować zapisy i odczyty z postgres-a
  
- dokładne czytanie kodu
   - wnikać w bardziej skomplikowany składniowo kod - weryfikacja jak gpchat tłumaczy (np mechanizm Specification), odpalić test dla takiego kodu
   - identyfikacja użytych building blocks: taktyczne ddd, cqrs-a, clean-architecture, crud (serwis + anemiczna encja),warstwy (na miro "Taktyka - Fundamenty modelu)
      - value object
      - encja (anemiczna bądź nie)
      - klucz (naturalny, techniczny)
      - agregat (granica zestawu danych, które muszą być spójne natychmiast wraz z regułami i enkapsulacją pilnujących tą spójność); reguła biznesowa; uważać na za szeroko zakrojoną granicę agregatu
      - problem z reguła biznesową w serwisie, nie ma scentralizowanego miejsca sprawdzenia reguły (jakim jest agregat).
      - 


- 
- persystencja agregagta
   - mapowanie agregat - relacje (na bazie) (...Configuration)
   - 
- analiza jak zaciagane są relacje encji reprezentującej agregagt w repozytoriach i jak są dla nich zrobione konfiguracje mappingu obiekt-tabela
    - include bez virtual chyba, chodzi o zaciągnięcie agregagtu jako całosći
    - jak następuje ich aktulizacja
    - poptrzec tez na podejscia na modularny monolit i solid webapi;
    - w ddd to tabela chage-request i jej relacje
    - alternatywa to josnowa kolumna