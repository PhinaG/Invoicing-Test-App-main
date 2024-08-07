<template>
   <br>
  <div class="about">   
    <router-link :to="{ name: 'Invoices' }">Back</router-link>
    
    <div class="container">      
     +Coupon: <input class="entry" type="text" name="invoice" v-model="state.couponCode" label="Coupon Code" placeholder="Coupon code" />
     <button class="entry" style="background-color: rgb(67, 179, 67); color:white" @click="updateInvoice($route.params.id)">Update Invoice</button>
    </div>



    <h2>Invoice Details</h2>
    <div class="container">
      <div class="entry">Invoice #{{$route.params.id}}</div>
      <div class="spacer"></div>
      <div class="entry">Client : {{$route.params.clientName}} </div>
    </div>    

    <h3>Line Items</h3>

    <table>
      <thead>
        <th>ID</th>
        <th>Description</th>
        <th>Quantity</th>
        <th>Cost</th>
        <th>Billable</th>
        <th></th>
      </thead>
      <tbody>
        <tr v-for="item in state.lineItems.lineItem" :key="item.id">
          <td>{{item.id}}</td>
          <td>{{item.description}}</td>
          <td>{{item.quantity}}</td>
          <td>{{item.cost}}</td>
           <td>  
            <input type="checkbox" :id="item.id" @change="handleBillableStatus($event)" :name="item.invoiceId" :value="item.isBillable" :checked = "item.isBillable ? true : false" >
            
          </td>
          <td>  
            <button :id="item.id" @click="deleteItem(item.id)" class="error-button">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

<div class='text-right'><strong>Total Value : </strong> {{state.lineItems.grandTotal}}</div>
<div class='text-right'><strong>Total Billable Value : </strong> {{state.lineItems.totalBillableValue}}</div>

    <form @submit.prevent>
      <h4>Create Line Item</h4>

      <div class="container">
        <input class="entry" type="text" name="description" placeholder="Description" v-model="state.description" />
      Quantity: <input class="entry"  type="number" name="quantity" placeholder=   "Quantity" v-model="state.quantity" />
      Cost:<input class="entry" type="number" name="cost" placeholder="Cost" v-model="state.cost" />
    </div>
    <div class="container">
      Billable: <input type="checkbox" name="isbillable"  v-model="state.isbillable" />
      <button  @click="createLineItem" class="create-button">Add Item to Invoice</button>
    </div>
     
    </form>
  </div>
</template>

<style scoped>
.container {
  display: flex; /* Align entries vertically */
  align-items: center;    /* Center horizontally */
}

.entry {
  background-color: #f0f0f0;
  border: 1px solid #ccc;
  padding: 6px;
  margin: 8px;
  width: 80%;
  text-align: center;
}

.spacer {
  flex: 1;/* Adjust spacer height as needed */
}


.create-button{
    color:white; 
    background-color: rgb(73, 73, 194);
  }
  
.success-button{
  color:white; 
  background-color:rgb(67, 179, 67);
}

.error-button{
  color:white; 
  background-color:rgb(211, 86, 86);
}

.create-button:hover {
  background-color: #6f7275;
}

.success-button:hover {
  background-color: #6f7275;
}

.error-button:hover {
  background-color: #6f7275;
}
</style>

<script lang="ts">
import {defineComponent, onMounted, reactive} from "vue";

export default defineComponent({
  name: "Invoice",
  props: {
    id: {
      type: [String, Number],
      required: true
    },
    clientName:{
      type: [String, Number],
      required: true
    },
    couponCode:{
      type: [String, Number]
    }
  },
  methods:{

    

    handleBillableStatus: function (event : any){
    
        let {value,id,name} = event.target ;
       // console.log(event.target.value);
       // console.log(event.target.id);

    let newValue = false;

        if(value == "true"){
          newValue = false;
        }else if(value == "false"){
          newValue = true;
        }

       // console.log(newValue);


             fetch(`http://localhost:5000/invoices/Update/`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          lineItemId:parseInt(id),
          isBillable:newValue,
          invoiceId: parseInt(name)// pass the acctual selected InvoiceId 
        })
      }).then(() => {
        window.location.reload();
      });
    }
  },
  setup(props) {
    const state = reactive({
      lineItems: [],
      description: "",
      quantity: "0",
      cost: "0",
      isbillable: true,
      invoiceId: props.id,
      couponCode:props.couponCode
    })

    function fetchLineItems(message: string = '') {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        if(response.ok){
          response.json().then(lineItems => (state.lineItems = lineItems))
          if(message != ''){
            alert(message);
          }
        }
        
      })
    }

    function createLineItem() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          description: state.description,
          quantity: Number(state.quantity),
          cost: Number(state.cost),
          isBillable: state.isbillable
        })
      }).then((response) => {
        
        if(response.ok){         
          fetchLineItems(`A line item has been added to the invoice`);
        }
      })
    }

    function deleteItem(id:Number){
      fetch(`http://localhost:5000/invoices/${props.id}/items/${id}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json"
        }
      }).then((response) => {
        
        if(response.ok){         
          fetchLineItems(`A line item with ID: ${id} has been removed from the invoice`);
        }
      })
    }

    function updateInvoice() {
      fetch(`http://localhost:5000/invoices/${props.id}`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          couponCode: state.couponCode
        })
      }).then((response) => {
        if(response.ok){
          alert(`Coupon code: ${state.couponCode} has been updated to the invoice`)
        }
      })
    }

    onMounted(fetchLineItems)

    return {state, createLineItem, deleteItem, updateInvoice}
  }
})
</script>

