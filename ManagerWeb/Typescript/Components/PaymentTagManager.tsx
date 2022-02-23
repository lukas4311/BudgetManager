import React from 'react'

class TagComponentState {
    tags: string[];
    tagName: string;
}

class TagComponentProps {
    tags: string[];
    tagsChange: (tags: string[]) => void;
}

export default class PaymentTagManager extends React.Component<TagComponentProps, TagComponentState>{

    constructor(props: TagComponentProps) {
        super(props);
        this.state = { tags: props.tags, tagName: "" }
    }

    private tagInputLost = () => {
        let tags: string[] = this.state.tags;
        tags.push(this.state.tagName);
        this.setState({ tags: tags, tagName: "" });
        this.props.tagsChange(tags);
    }

    private handleChangeName = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ tagName: e.target.value });
    }

    componentDidUpdate(prevProps: TagComponentProps) {
        if (prevProps.tags !== this.props.tags) {
            this.setState({ tags: this.props.tags });
        }
    }

    private deleteTag = (event: React.MouseEvent<HTMLSpanElement, MouseEvent>, tagName: string) => {
        let tags = this.state.tags;
        let index = tags.indexOf(tagName);

        if (index != -1) {
            tags.splice(index, 1);
            this.setState({ tags: tags });
            this.props.tagsChange(tags);
        }
    };

    render() {
        return (
            <div className="flex mb-4">
                <div className="w-7/10 pl-6 text-left">
                    {this.state.tags.map(t => (
                        <div key={t} className="bg-battleshipGrey inline rounded-sm mr-2">
                            <span className="mr-4 mb-1 ml-1">{t}</span>
                            <span className="closeTag align-text-top mr-1 mt-1 cursor-pointer" onClick={(e) => this.deleteTag(e, t)}>X</span>
                        </div>
                    ))}
                </div>
                <div className="w-3/10 pr-6">
                    <input className="text-black border-1 border-white rounded-md w-32 right" onBlur={this.tagInputLost} value={this.state.tagName} onChange={this.handleChangeName}></input>
                </div>
            </div>)
    }
}