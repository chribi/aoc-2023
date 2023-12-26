using System.Text;
using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> lines) {
    var stones = lines.Select(ReadLine).ToArray();
    var count = 0;
    for (var i = 0; i < stones.Length; i++)
        for (var j = i + 1; j < stones.Length; j++) {
            var intersection = IntersectXY(stones[i], stones[j]);
            if (intersection == null) continue;
            var (x, y) = intersection.Value;
            Log(x,y , stones[i], stones[j]);
            if (2e14 <= x && x <= 4e14 && 2e14 <= y && y <= 4e14)
                count++;
        }
    return count;
}

long SolvePart2(IEnumerable<string> lines) {
    var stones = lines.Select(ReadLine).ToArray();
    var (m, b) = GetLinearEquations(stones);
    Console.WriteLine("Linear equations");
    Print();

    Console.WriteLine("Gauss elimination");
    GaussianElimination(m, b);
    Print();

    Console.WriteLine("Normalization");
    Normalize(m, b);
    Print();

    var solution = RoundUp(b);

    return solution.X + solution.Y + solution.Z;

    void Print() {
        Console.WriteLine("m");
        PrintMatrix(m);
        Console.WriteLine("b");
        PrintVector(b);
    }
}

Hailstone RoundUp(double[] b) {
    // Let's just hope we didn't loose to much precision doing math
    // with doubles having big differences in orders of magnitude ...
    var px = Convert.ToInt64(b[0]);
    var py = Convert.ToInt64(b[1]);
    var pz = Convert.ToInt64(b[2]);
    var vx = Convert.ToInt64(b[3]);
    var vy = Convert.ToInt64(b[4]);
    var vz = Convert.ToInt64(b[5]);
    return new Hailstone(px, py, pz, vx, vy, vz);
}

(double, double)? IntersectXY(Hailstone a, Hailstone b) {
    var d = Det(a.Vx, b.Vx, a.Vy, b.Vy);
    if (Math.Abs(d) < 1e-8) return null;
    var t1 = Det(b.X - a.X, b.Vx, b.Y - a.Y, b.Vy) / d;
    var t2 = Det(b.X - a.X, a.Vx, b.Y - a.Y, a.Vy) / d;
    if (t1 <= 0 || t2 <= 0) return null;
    return (a.X + t1 * a.Vx, a.Y + t1 * a.Vy);
}

double Det(double a, double b, double c, double d)
    => a * d - b * c;

Hailstone ReadLine(string line) {
    var nums = ParseUtils.GetNumbers(line);
    return new(nums[0], nums[1], nums[2], nums[3], nums[4], nums[5]);
}

/*
 * Part 2: If the solution rock has position p and velocity v we get
 * for each hailstone s and some t the equation:
 *    p + t * v = p_s + t * v_s
 * After reordering
 *    p - p[s] = t * (v[s] - v)
 * therefore (p - p[s]) and (v[s] - v) are parallel.
 * => The determinant of the following matrix is 0
 *   [ (p_x - p[s]_x    (v[s]_x - v_x) ]
 *   [ (p_y - p[s]_y    (v[s]_y - v_y) ]
 * (Call this A[s])
 * This determinant contains products p_x*v_y, p_y*v_x of unknowns,
 * making the resulting equations non-linear.
 * But, when subtracting the equations for two hailstones a and b,
 * these non-linear terms cancel out.
 * By using different hailstone pairs we get enough linear equations that should
 * give as p_x, p_y, v_x, v_y:
 *   1: det(A[a]) - det(A[b]) = 0
 *   2: det(A[b]) - det(A[c]) = 0
 *   3: det(A[c]) - det(A[d]) = 0
 *   4: det(A[d]) - det(A[e]) = 0
 * We can use the same for also adding equations containing p_z and v_z.
 */
(double[,], double[]) GetLinearEquations(Hailstone[] hailstones) {
    // m * (p_x, p_y, p_z v_x, v_y, v_z) = c
    var m = new double[6, 6];
    var c = new double[6];
    FillRow(0, hailstones[0], hailstones[1]);
    FillRow(1, hailstones[1], hailstones[2]);
    FillRow(2, hailstones[2], hailstones[3]);
    FillRow(3, hailstones[3], hailstones[4]);
    FillZRow(4, hailstones[0], hailstones[1]);
    FillZRow(5, hailstones[1], hailstones[2]);
    return (m, c);

    void FillRow(int i, Hailstone a, Hailstone b) {
        m[i, 0] = a.Vy - b.Vy;
        m[i, 1] = b.Vx - a.Vx;
        m[i, 3] = b.Y - a.Y;
        m[i, 4] = a.X - b.X;
        c[i] = a.X * a.Vy - a.Y * a.Vx - b.X * b.Vy + b.Y * b.Vx;
    }
    void FillZRow(int i, Hailstone a, Hailstone b) {
        m[i, 0] = a.Vz - b.Vz;
        m[i, 2] = b.Vx - a.Vx;
        m[i, 3] = b.Z - a.Z;
        m[i, 5] = a.X - b.X;
        c[i] = a.X * a.Vz - a.Z * a.Vx - b.X * b.Vz + b.Z * b.Vx;
    }
}

// Why use a library when we can also use do a bad implementation ...
void GaussianElimination(double[,] m, double[] b) {
    var row = 0;
    var col = 0;
    var numRows = m.GetLength(0);
    var numCols = m.GetLength(1);
    while (row < numRows && col < numCols)
    {
        var iMax = Enumerable.Range(row, numRows - row)
            .MaxBy(r => Math.Abs(m[r, col]));
        if (Math.Abs(m[iMax, col]) < 1e-8) {
            col++;
            continue;
        }

        SwapRows(row, iMax, m, b);
        for (var i = row + 1; i < numRows; i++) {
            var f = -m[i, col] / m[row, col];
            AddRowMultiple(i, row, f, m, b);
        }
        row++;
        col++;
    }
}

void Normalize(double[,] m, double[] b) {
    for (var i = b.Length - 1; i >= 0; i--) {
        ScaleRow(i, 1 / m[i,i], m, b);
        for (var j = i + 1; j < b.Length; j++) {
            AddRowMultiple(i, j, -m[i, j], m, b);
        }
    }
}

void SwapRows(int i, int j, double[,] m, double[] b) {
    double tmp;
    for (var k = 0; k < m.GetLength(1); k++) {
        tmp = m[i, k];
        m[i, k] = m[j, k];
        m[j, k] = tmp;
    }
    tmp = b[i];
    b[i] = b[j];
    b[j] = tmp;

}

void ScaleRow(int i, double f, double[,] m, double[] b) {
    for (var j = 0; j < m.GetLength(1); j++)
        m[i, j] *= f;
    b[i] *= f;
}

void AddRowMultiple(int i, int j, double f, double[,] m, double[] b) {
    for (var k = 0; k < m.GetLength(1); k++)
        m[i, k] += m[j, k] * f;
    b[i] += b[j] * f;
}

void PrintMatrix(double[,] m) {
    for (var row = 0; row < m.GetLength(0); row++)
    {
        var s = new StringBuilder();
        for (var col = 0; col < m.GetLength(1); col++) {
            s.AppendFormat("{0:0.00}\t", m[row, col]);
        }
        Console.WriteLine(s);
    }
}

void PrintVector(double[] b) {
    for (var i = 0; i < b.Length; i++)
        Console.WriteLine(b[i]);
}


if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

record struct Pos(long X, long Y, long Z);
record struct Hailstone(long X, long Y, long Z,
        long Vx, long Vy, long Vz);
