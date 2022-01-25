(defn exp [x, n]    ;; Calculates e^x
  (reduce * (repeat n, x)))

(defn square [x]
  (* x, x))

(defn calculate-step [a, b, n]
  (/ (- b, a), n))

(defn calculate-first-term [f, x0, xn]
  (* (+ (f x0), (f xn)), 0.5))

(defn calc [f, a, start, end, step]
  (loop [x, start, result, 0]
    (if (>= x, end)
      result
      (recur (+ x step) (+ result (f x))))))

(defn take-integral [f, a, b, n, h]  ;; Takes integral of function f
  (* h, (+ (calculate-first-term f, a, b), (calc f, a, (+ a, h), b, h))))

(def calc-memo
  (memoize (fn [f, a, start, end, step]
             (loop [x, start, result, 0]
               (if (>= x end)
                 result
                 (recur (+ x, step), (+ result, (f x))))))))

(def take-integral-memo  ;; Memoized integral
  (memoize (fn [f, a, b, n, h] (* h, (+ (calculate-first-term f, a, b), (calc-memo f, a, (+ a, h), b, h))))))

(do
  (time (take-integral square 0. 1. 5 (calculate-step 0. 1. 5)))
  (time (take-integral-memo square 0. 1. 5 (calculate-step 0. 1. 5)))
  (time (take-integral-memo square 0. 1. 5 (calculate-step 0. 1. 5))))