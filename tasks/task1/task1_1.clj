(defn add [symbol, words]
  (if (empty? (first words))
    () ; then
    (if (str/starts-with? (first words), symbol) ; else
      (add symbol, (rest words)) ; then
      (cons (str symbol, (first words)), (add symbol, (rest words)))))) ; else

(defn ads [symbol, words]
  (if(empty? (first symbol))
  () ; then
  (concat
   (add (first symbol), words)
   (ads (rest symbol), words)))) ; else

(defn task1 [symbol, n]
  (loop [n, (dec n), words, symbol]
    (if (> n, 0)
      (recur (dec n), (ads symbol, words)) ; then
      words))) ; else

(task1 ["a", "b", "c"], 3)