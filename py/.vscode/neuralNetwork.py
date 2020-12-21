from numpy.testing._private.utils import nulp_diff
import scipy.special
import matplotlib.pyplot
import numpy
import matplotlib.pyplot

class neuralNetwork:
    def __init__(
        self,
        inputnodes,
        hiddennodes,
        outputnodes,
        learningrate
        ):

        self.inodes = inputnodes;
        self.hnodes = hiddennodes;
        self.onodes = outputnodes;
        self.lr = learningrate;

        self.wih = numpy.random.normal(0.0, pow(self.hnodes, -0.5),(self.hnodes, self.inodes))
        self.who = numpy.random.normal(0.0, pow(self.onodes, -0.5),(self.onodes, self.hnodes))

        self.activation_function = lambda x: scipy.special.expit(x)
        pass

    def train(self, inputs_list, targets_list):
        inputs = numpy.array(inputs_list, ndmin=2).T
        targets = numpy.array(targets_list, ndmin=2).T

        hidden_inputs = numpy.dot(self.wih, inputs)
        hidden_outputs = self.activation_function(hidden_inputs);
        final_inputs = numpy.dot(self.who, hidden_outputs)
        final_outputs = self.activation_function(final_inputs)

        output_errors = targets - final_outputs
        hidden_errors = numpy.dot(self.who.T, output_errors)

        self.who += self.lr * numpy.dot((output_errors * final_outputs *( 1.0 - final_outputs)), numpy.transpose(hidden_outputs))
        self.wih += self.lr * numpy.dot((hidden_errors * hidden_outputs *(1.0 - hidden_outputs)), numpy.transpose(inputs))
        pass
    
    def query(self, inputs_list):
        inputs = numpy.array(inputs_list, ndmin=2).T
        hidden_inputs = numpy.dot(self.wih, inputs);
        hidden_outputs = self.activation_function(hidden_inputs)
        final_inputs = numpy.dot(self.who, hidden_outputs);
        final_outputs = self.activation_function(final_inputs)

        return final_outputs

#helper methods
def plotLetter(array):
    all_values = array.split(',')
    image_array = numpy.asfarray(all_values[1:]).reshape((28,28))
    matplotlib.pyplot.imshow(image_array, cmap="Greys", interpolation="None")
    matplotlib.pyplot.show();
    pass

def trainWithData(array, output_nodes, neuralNetwork):
    all_values = array.split(',')
    input =(numpy.asfarray(all_values[1:]) / 255.0 * 0.99) + 0.01
    targets = numpy.zeros(output_nodes) + 0.01
    targets[int(all_values[0])] = 0.99
    neuralNetwork.train(input, targets)
    pass;

def adjustBytesTo0To1(all_values):
    input =((numpy.asfarray(all_values[1:]) / 255.0 * 0.99)+ 0.01)
    return input;


#Train
input_nodes = 784
hidden_nodes = 100
output_nodes = 10
learning_rate = 0.3

n = neuralNetwork(input_nodes, hidden_nodes, output_nodes, learning_rate)

data_path = r"C:\test_data\mnist_train_100.csv"
data_file = open(data_path, 'r')
data_list = data_file.readlines()
data_file.close()

for x in data_list:
    trainWithData(x, output_nodes, n)


print("done training")

#Query

data_path = r"C:\test_data\mnist_test_10.csv"
data_file = open(data_path, 'r')
data_list = data_file.readlines()
data_file.close()
allValues = data_list[0].split(',')
print(allValues[0]);
input = adjustBytesTo0To1(allValues);

output = n.query(inputs_list=input)
print(output)
