import React from 'react'

class TagComponentState {
    tags: string[]
}

class TagComponentProps{
    tags: string[]
}

export default class PaymentTagManager extends React.Component<TagComponentProps, TagComponentState>{
    constructor(props: TagComponentProps) {
        super(props);
        this.state = { tags: props.tags }
    }

    render() {
        return (<div>
            <p>Tag komponenta</p>
            {this.state.tags.map(t => (
                <div><span>{t}</span><span>X</span></div>
            ))}
        </div>)
    }
}